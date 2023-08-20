using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class PhysicsManager : SingletonComponent<PhysicsManager>
{
    Dictionary<Collider2D, PlatformEffector2D> m_PlatformEffectorCache = new Dictionary<Collider2D, PlatformEffector2D>();

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
        PopulateColliderDictionary(ref m_PlatformEffectorCache);
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

    public bool ColliderHasPlatformEffector(Collider2D collider)
    {
        return Instance.m_PlatformEffectorCache.ContainsKey(collider);
    }

    public bool TryGetPlatformEffector(Collider2D collider, out PlatformEffector2D platformEffector)
    {
        return Instance.m_PlatformEffectorCache.TryGetValue(collider, out platformEffector);
    }
}