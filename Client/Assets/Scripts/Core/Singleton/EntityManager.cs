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
    public CharacterComponent PlayerCharacter
    {
        get;
        private set;
    }

    public CharacterComponent BossCharacter
    {
        get;
        private set;
    }
    
    public IEnumerable<CharacterComponent> Monsters
    {
        get
        {
            return entityDictionary.Values.Where(e => e.IsPlayer == false).OfType<CharacterComponent>();
        }
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


    public IEnumerator LoadAsyncMonsters(IEnumerable<ENUM_ENTITY_TYPE> entityTypes)
    {
        foreach(var entityType in entityTypes)
        {
            yield return LoadAsyncEntity(entityType, false, false);
        }
    }

    public IEnumerator LoadAsyncPlayer(ENUM_ENTITY_TYPE entityType)
    {
        yield return LoadAsyncEntity(entityType, true, false);
    }

    public IEnumerator LoadAsyncBoss(ENUM_ENTITY_TYPE entityType)
    {
        yield return LoadAsyncEntity(entityType, false, true);
    }

    private IEnumerator LoadAsyncEntity(ENUM_ENTITY_TYPE characterType, bool isPlayer, bool isBoss)
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

		var entity = obj.GetComponent<CharacterComponent>();
        if (entity == null)
            yield break;

        int ownerGuid = GameConfig.MonsterGuid;
        ENUM_LAYER_TYPE layerType = ENUM_LAYER_TYPE.Enemy;

        if (isPlayer)
        {
            ownerGuid = GameConfig.PlayerGuid;
            layerType = ENUM_LAYER_TYPE.Friendly;

            PlayerCharacter = entity;
        }
        else if(isBoss)
        {
            layerType = ENUM_LAYER_TYPE.Boss;

            BossCharacter = entity;
        }

        entity.Initialize(ownerGuid, characterType, isPlayer);
        entity.SetEntityLayer(layerType);
        mono.SetSingletonChild(this, entity);
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
        return GetXDistance(fromEntity, PlayerCharacter);
    }

}
