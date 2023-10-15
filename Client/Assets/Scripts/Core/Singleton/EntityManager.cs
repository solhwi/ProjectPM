using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class EntityManager : Singleton<EntityManager>
{
    public CharacterComponent PlayerEntity
    {
        get;
        private set;
    }
    
    public CharacterComponent MonsterEntity
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

    public IEnumerator LoadAsyncMonster(ENUM_ENTITY_TYPE characterType)
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

        MonsterEntity = obj.GetComponent<CharacterComponent>();
        if (MonsterEntity == null)
            yield break;

        MonsterEntity.Initialize(0, characterType, false);
        MonsterEntity.SetEntityLayer(ENUM_LAYER_TYPE.Enemy);

        mono.SetSingletonChild(this, MonsterEntity);
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

		PlayerEntity.Initialize(GameManager.Instance.PlayerGuid, characterType, true);
		PlayerEntity.SetEntityLayer(ENUM_LAYER_TYPE.Friendly);

		mono.SetSingletonChild(this, PlayerEntity);
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


    private IEnumerable<EntityComponent> GetOverlapEntities(int entityGuid, Vector3 pos, Vector3 size, Vector3 offset, Vector3 velocity, bool includeMine = false)
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

            if (includeMine == false && entity.OwnerGuid == entityGuid)
                continue;
                
			yield return entity;
		}
    }

    private IEnumerable<EntityComponent> GetOverlapEntities(FrameEntityMessage message, bool includeMine)
    {
        return GetOverlapEntities(message.entityGuid, message.pos, message.hitbox, message.offset, message.velocity, includeMine);
    }

    public Dictionary<ENUM_SKILL_TYPE, IEnumerable<EntityComponent>> GetSearchedEntities(EntityComponent entity, bool includeMine = false)
    {
        var skillTable = ScriptParserManager.Instance.GetTable<CharacterSkillTable>();
        if (skillTable == null)
            return null;

        var searchedEntities = new Dictionary<ENUM_SKILL_TYPE, IEnumerable<EntityComponent>>();

        var hasSkills = skillTable.GetSkills(entity.EntityType);
        foreach(var skill in hasSkills)
        {
            Vector2 box = new(skill.searchBoxX, skill.searchBoxY);
            Vector2 offset = new(skill.searchOffsetX, skill.searchOffsetY);

            var entities = GetOverlapEntities(entity.Guid, entity.Position, box, offset, entity.Velocity, includeMine);
            if(entities == null || entities.Any() == false) 
                continue;

            searchedEntities.Add(skill.key, entities);
        }

        return searchedEntities;
    }

    public IEnumerable<EntityComponent> GetOverlapEntities(EntityComponent entity, bool includeMine = false)
    {
        return GetOverlapEntities(entity.Guid, entity.Position, entity.HitBox, entity.HitOffset, entity.Velocity, includeMine);
    }

    public IEnumerable<int> GetOverlapEntitiyGuids(FrameEntityMessage message, bool includeMine = false)
    {
        return GetOverlapEntities(message, includeMine).Select(entity => entity.Guid);
    }

}
