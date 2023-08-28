using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameColliderData
{
    public readonly AttackableComponent attacker;
	public readonly DamageableComponent defender;
    public readonly int frameCount;

    public FrameColliderData(AttackableComponent attacker, DamageableComponent defender, int frameCount)
    {
        this.attacker = attacker;
        this.defender = defender;
        this.frameCount = frameCount;
    }
}

public class PhysicsManager : SingletonComponent<PhysicsManager>
{
    Dictionary<Collider2D, AttackableComponent> attackableCache = new Dictionary<Collider2D, AttackableComponent>();
    Queue<FrameColliderData> inputDataQueue = new Queue<FrameColliderData>();

    protected override void OnAwakeInstance()
    {
        SceneManager.Instance.onSceneChanged += PopulateCollider;
        PopulateCollider(SceneType.Title);
    }

    protected override void OnReleaseInstance()
    {
        SceneManager.Instance.onSceneChanged -= PopulateCollider;
    }

    private void PopulateCollider(SceneType type)
    {
        PopulateColliderDictionary(ref attackableCache);
    }

    private void PopulateColliderDictionary<TComponent>(ref Dictionary<Collider2D, TComponent> dict)
        where TComponent : Component
    {
        dict.Clear();

        TComponent[] components = FindObjectsOfType<TComponent>();

        for (int i = 0; i < components.Length; i++)
        {
            Collider2D[] componentColliders = components[i].GetComponents<Collider2D>();

            for (int j = 0; j < componentColliders.Length; j++)
            {
                dict.Add(componentColliders[j], components[i]);
            }
        }
    }

	private void Update()
	{
		int validFrameCount = Time.frameCount;

        Dictionary<DamageableComponent, List<AttackableComponent>> damageableDictionary = new Dictionary<DamageableComponent, List<AttackableComponent>>();
		
        while (inputDataQueue.TryDequeue(out var result))
		{
			if (result.frameCount < validFrameCount)
				continue;

            if (damageableDictionary.ContainsKey(result.defender))
            {
				damageableDictionary[result.defender].Add(result.attacker);
			}
            else
            {
				damageableDictionary.Add(result.defender, new List<AttackableComponent> { result.attacker });
			}
		}

		foreach(var damagePair in damageableDictionary)
        {
            var damageable = damagePair.Key;
            var attackers = damagePair.Value;

            damageable.OnDamage(attackers);
        }
	}

	public void OnTrigger(Collider2D attackerCollider, DamageableComponent defender)
	{
        if(attackableCache.TryGetValue(attackerCollider, out var attacker))
        {
            inputDataQueue.Enqueue(new FrameColliderData(attacker, defender, Time.frameCount));
		}
	}
}