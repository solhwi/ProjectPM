using Mirror;
using NPOI.HPSF;
using StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class EntityManager : Singleton<EntityManager>
{
    public CharacterComponent PlayerEntity
    {
        get;
        private set;
    }

    private Dictionary<int, EntityComponent> entityDictionary = new Dictionary<int, EntityComponent>();
    private Dictionary<Collider2D, EntityComponent> colliderDictionary = new Dictionary<Collider2D, EntityComponent>();
	
	public int Register(EntityComponent objectComponent)
	{
		int Guid = objectComponent.GetInstanceID();
		entityDictionary[Guid] = objectComponent;
		return Guid;
	}

	public int UnRegister(int Guid)
	{
		if (entityDictionary.ContainsKey(Guid))
			entityDictionary.Remove(Guid);

        return 0;
	}

    // 투사체들의 업데이트가 수행된다.
	public override void OnUpdate(int deltaFrameCount, float deltaTime)
	{
        foreach(var entity in entityDictionary.Values)
        {
            entity.OnUpdate();
        }
	}

	public EntityComponent GetEntityComponent(int guid)
    {
        if (entityDictionary.ContainsKey(guid) == false)
            return null;

        return entityDictionary[guid];
    }

    public IEnumerable<EntityComponent> GetAllEntities()
    {
        return entityDictionary.Values;
    }

    public IEnumerable<EntityComponent> GetEntities(int ownerGuid)
    {
        return GetAllEntities().Where(e => e.OwnerGuid == ownerGuid);
    }

    public IEnumerator LoadAsyncPlayer(ENUM_ENTITY_TYPE characterType)
    {
        var handle = ResourceManager.Instance.LoadAsync<CharacterComponent>();
        while (!handle.IsDone || handle.Status != AsyncOperationStatus.Succeeded)
        {
            yield return null;
        }

        var prefab = handle.Result as GameObject;
        if (prefab == null)
            yield break;

        var obj = UnityEngine.Object.Instantiate(prefab);
        if (obj == null)
            yield break;

		PlayerEntity = obj.GetComponent<CharacterComponent>();
        if (PlayerEntity == null)
            yield break;

		PlayerEntity.Initialize(GameManager.Instance.PlayerGuid, characterType);
		PlayerEntity.SetEntityLayer(ENUM_LAYER_TYPE.Friendly);

		mono.SetSingletonChild(this, PlayerEntity);
    }

	private IEnumerable<EntityComponent> GetOverlapEntities(int entityGuid, Vector3 pos, Vector3 size, Vector3 offset, Vector3 velocity)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(pos + offset + velocity, size, 0);
        if (colliders == null || colliders.Length == 0)
            yield break;

        var myEntity = GetEntityComponent(entityGuid);
        if (myEntity == null)
            yield break;
        
        foreach (var collider in colliders)
        {
            if (colliderDictionary.TryGetValue(collider, out var entity) == false)
            {
				entity = collider.GetComponent<EntityComponent>();
				colliderDictionary.Add(collider, entity);
			}

			if (entity == null)
				continue;

			if (entity.OwnerGuid == myEntity.OwnerGuid) // 내껀 제외
				continue;

			yield return entity;
		}
    }

    private IEnumerable<EntityComponent> GetOverlapEntities(FrameEntityMessage message)
    {
        return GetOverlapEntities(message.entityGuid, message.pos, message.hitbox, message.offset, message.velocity);
    }

    public IEnumerable<int> GetOverlapEntitiyGuids(FrameEntityMessage message)
    {
        return GetOverlapEntities(message).Select(entity => entity.Guid);
    }
}
