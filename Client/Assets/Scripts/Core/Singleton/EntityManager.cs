using NPOI.SS.Formula.Functions;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.RuleTile.TilingRuleOutput;

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
        Collider2D[] colliders = PhysicsManager.Instance.OverlapBoxAll(pos + offset + velocity, size);
        if (colliders == null || colliders.Length == 0)
            yield break;

        var myEntity = GetEntityComponent(entityGuid);
        if (myEntity == null)
            yield break;
        
        foreach (var collider in colliders)
        {
            var entity = collider.GetComponent<EntityComponent>();
            if (entity == null)
				continue;

            if (includeMine == false && entity.OwnerGuid == myEntity.OwnerGuid)
                continue;
                
			yield return entity;
		}
    }

    private IEnumerable<EntityComponent> GetOverlapEntities(FrameEntityMessage message, bool includeMine)
    {
        return GetOverlapEntities(message.entityGuid, message.pos, message.hitbox, message.offset, message.velocity, includeMine);
    }

    public IEnumerable<EntityComponent> GetSearchedEntities(EntityComponent entity, ENUM_SKILL_TYPE skillType, bool includeMine = false)
    {
        var skillTable = ScriptParserManager.Instance.GetTable<CharacterSkillTable>();
        if (skillTable == null)
            return null;

        var hasSkill = skillTable.GetSkillInfo(skillType);
        if (hasSkill == null)
            return null;

        Vector2 box = new(hasSkill.searchBoxX, hasSkill.searchBoxY);
        Vector2 offset = new(hasSkill.searchOffsetX, hasSkill.searchOffsetY);

        return GetOverlapEntities(entity.Guid, entity.Position, box, offset, entity.Velocity, includeMine);
    }

    public IEnumerable<EntityComponent> GetOverlapEntities(EntityComponent entity, bool includeMine = false)
    {
        return GetOverlapEntities(entity.Guid, entity.Position, entity.HitBox, entity.HitOffset, entity.Velocity, includeMine);
    }

    public IEnumerable<int> GetOverlapEntitiyGuids(FrameEntityMessage message, bool includeMine = false)
    {
        return GetOverlapEntities(message, includeMine).Select(entity => entity.Guid);
    }

    public float GetXDistance(EntityComponent fromEntity, EntityComponent toEntity)
    {
       return toEntity.Position.x - fromEntity.Position.x;
    }

    public float GetXDistanceFromPlayer(EntityComponent fromEntity)
    {
        return GetXDistance(fromEntity, PlayerEntity);
    }

}
