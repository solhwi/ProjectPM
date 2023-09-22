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
        SceneManager.Instance.onSceneChanged += OnChangedScene;
        OnChangedScene(SceneType.Title);
    }

    protected override void OnReleaseInstance()
    {
        SceneManager.Instance.onSceneChanged -= OnChangedScene;
    }

    private void OnChangedScene(SceneType type)
    {
        attackableCache.Clear();
        inputDataQueue.Clear();
	}

    public void RegisterCollider(Collider2D collider, AttackableComponent attackable)
    {
        if(attackableCache.ContainsKey(collider) == false)
		    attackableCache.Add(collider, attackable);
	}

	public void UnRegisterCollider(Collider2D collider)
	{
		if (attackableCache.ContainsKey(collider))
			attackableCache.Remove(collider);
	}

	public override void OnUpdate(int deltaFrameCount, float deltaTime)
	{
		int validFrameCount = Time.frameCount;

        var damageableDictionary = new Dictionary<DamageableComponent, List<AttackableComponent>>();
		
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