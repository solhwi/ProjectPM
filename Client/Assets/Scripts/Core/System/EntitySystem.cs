using Cysharp.Threading.Tasks;
using NPOI.SS.Formula.Functions;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class EntitySystemExtension
{
	public static IEnumerable<int> GetOverlapEntityGuids(this EntitySystem system, FrameEntityMessage message, bool includeMine = false)
	{
		return system.GetOverlapEntities(message, includeMine).Select(entity => entity.EntityGuid);
	}

	public static IEnumerable<IEntity> GetOverlapEntities(this EntitySystem system, FrameEntityMessage message, bool includeMine)
	{
		return system.GetOverlapEntities(message.entityGuid, message.pos, message.hitbox, message.offset, message.velocity, includeMine);
	}

	public static IEnumerable<IEntity> GetSearchedEntities(this EntitySystem system, IEntity entity, ENUM_SKILL_TYPE skillType, bool includeMine = false)
	{
        //var skillTable = ScriptParsingSystem.Instance.GetTable<CharacterSkillTable>();
        //if (skillTable == null)
        //	return null;

        //var hasSkill = skillTable.GetSkillInfo(skillType);
        //if (hasSkill == null)
        //	return null;

        //Vector2 box = new(hasSkill.searchBoxX, hasSkill.searchBoxY);
        //Vector2 offset = new(hasSkill.searchOffsetX, hasSkill.searchOffsetY);

        //return system.GetOverlapEntities(entity.EntityGuid, entity.Position, box, offset, entity.Velocity, includeMine);
        return null;
    }
}

public class EntitySystem : MonoSystem
{
    public IEntity Player
    {
        get
        {
            return entityDictionary.Values.FirstOrDefault(e => e.IsPlayer);
        }
    }

    public IEntity Boss
    {
        get
        {
            return entityDictionary.Values.FirstOrDefault(e => e.IsBoss);
        }
    }
    
    public IEnumerable<IEntity> Enemies
    {
        get
        {
            return entityDictionary.Values.Where(e => e.IsPlayer == false && e.IsBoss == false);
        }
    }

    [System.Serializable]
    private class EntityDictionary : SerializableDictionary<int, IEntity> { }
    [SerializeField]  private EntityDictionary entityDictionary = new();

    [SerializeField] private AddressableResourceSystem resourceSystem;
    [SerializeField] private EntityCollisionSubSystem collisionSubSystem;
    [SerializeField] private EntityControlSubSystem controlSubSystem;

    protected override void OnReset()
    {
        base.OnReset();

        resourceSystem = SystemHelper.GetSystemAsset<AddressableResourceSystem>();
        collisionSubSystem = SystemHelper.GetSystemAsset<EntityCollisionSubSystem>();
        controlSubSystem = SystemHelper.GetSystemAsset<EntityControlSubSystem>();
    }

    public override void OnUpdate(int deltaFrameCount, float deltaTime)
	{
		controlSubSystem.UpdateControl();
	}

	public IEnumerable<IEntity> GetOverlapEntities(int entityGuid, Vector3 pos, Vector3 size, Vector3 offset, Vector3 velocity, bool includeMine = false)
	{
        return collisionSubSystem.GetOverlapEntities(entityGuid, pos, size, offset, velocity, includeMine);
	}

	public void ToPlayerControl()
	{
		controlSubSystem.ToPlayerControl(Player);
	}

	public void ToAIControl()
	{
        foreach (var enemy in Enemies)
        {
			controlSubSystem.ToAIControl(enemy);
		}

        controlSubSystem.ToAIControl(Boss);
	}

	public void OnDrawGizmos()
	{
		collisionSubSystem.OnDrawGizmos();
	}

	public async UniTask<IEnumerable<IEntity>> CreateEnemies(IEnumerable<EnemySpawnData> spawnDatas)
    {
        var entities = new List<IEntity>();

        foreach(var data in spawnDatas)
        {
            entities.Add(await CreateEnemy(data.entityType));
		}

        return entities;
    }

    public async UniTask<IEntity> CreateEnemy(ENUM_ENTITY_TYPE entityType)
    {
        return await CreateEntity<CharacterBehaviour>(entityType, false, false);
	}

    public async UniTask<IEntity> CreatePlayer(ENUM_ENTITY_TYPE entityType)
    {
        return await CreateEntity<CharacterBehaviour>(entityType, true, false);
    }

    public async UniTask<IEntity> CreateBoss(EnemySpawnData spawnData)
    {
        return await CreateEntity<CharacterBehaviour>(spawnData.entityType, false, true);
    }

    private async UniTask<IEntity> CreateEntity<T>(ENUM_ENTITY_TYPE entityType, bool isPlayer, bool isBoss) where T : EntityBehaviour
    {
        var entityBehaviour = await resourceSystem.InstantiateAsync<T>();

#if UNITY_EDITOR
        entityBehaviour.name = entityType.ToString();
#endif

        int ownerGuid = GameConfig.MonsterGuid;
        ENUM_LAYER_TYPE layerType = ENUM_LAYER_TYPE.Enemy;

        if (isPlayer)
        {
            ownerGuid = GameConfig.PlayerGuid;
            layerType = ENUM_LAYER_TYPE.Friendly;
        }
        else if(isBoss)
        {
            layerType = ENUM_LAYER_TYPE.Boss;
        }

		int entityGuid = entityBehaviour.GetInstanceID();

		entityBehaviour.Initialize(ownerGuid, entityGuid, entityType, isPlayer);
        entityBehaviour.SetEntityLayer(layerType);

        SceneModuleSystemManager.Instance.SetSystemChild(this, entityBehaviour);

		entityDictionary[entityGuid] = entityBehaviour;
		return entityBehaviour;
    }

    public IEntity GetEntity(int guid)
    {
        if (entityDictionary.ContainsKey(guid) == false)
            return null;

        return entityDictionary[guid];
    }

    public IEnumerable<IEntity> GetAllEntities()
    {
        return entityDictionary.Values;
    }

    public IEnumerable<IEntity> GetEntities(int ownerGuid)
    {
        return GetAllEntities().Where(e => e.OwnerGuid == ownerGuid);
    }

	public Vector2 GetDistance(IEntity fromEntity, IEntity toEntity)
	{
		return toEntity.Position - fromEntity.Position;
	}

    public float GetDistanceX(IEntity fromEntity, IEntity toEntity)
    {
        return GetDistance(fromEntity, toEntity).x;
    }

    public Vector2 GetDistanceWithPlayer(IEntity fromEntity)
    {
        return GetDistance(fromEntity, Player);
    }

    public float GetDistanceXWithPlayer(IEntity fromEntity)
    {
        return GetDistanceWithPlayer(fromEntity).x;
    }
}
