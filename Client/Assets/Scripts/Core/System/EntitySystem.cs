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
	public static IEnumerable<int> GetAttackerEntityGuids(this EntitySystem system, FrameEntityMessage message)
	{
		return system.GetAttackerEntities(message).Select(entity => entity.EntityGuid);
	}

	public static IEnumerable<IEntity> GetAttackerEntities(this EntitySystem system, FrameEntityMessage message)
	{
		return system.GetAttackerEntities(message.entityGuid, message.pos, message.hitbox, message.offset, message.velocity);
	}

	public static IEnumerable<int> GetDamagerEntityGuids(this EntitySystem system, FrameEntityMessage message)
	{
		return system.GetDamagerEntities(message).Select(entity => entity.EntityGuid);
	}

	public static IEnumerable<IEntity> GetDamagerEntities(this EntitySystem system, FrameEntityMessage message)
	{
		return system.GetDamagerEntities(message.entityGuid, message.pos, message.hitbox, message.offset, message.velocity);
	}

	public static IEnumerable<IEntity> GetOverlapEnemies(this EntitySystem system, IEntity entity, Vector2 searchBox, Vector2 searchOffset)
	{
        return system.GetOverlapEnemies(entity.EntityGuid, entity.Position, searchBox, searchOffset, entity.Velocity);
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
    [SerializeField] private EntityComponentSystem componentSystem;

    protected override void OnReset()
    {
        base.OnReset();

        resourceSystem = AssetLoadHelper.GetSystemAsset<AddressableResourceSystem>();
        collisionSubSystem = AssetLoadHelper.GetSystemAsset<EntityCollisionSubSystem>();
        controlSubSystem = AssetLoadHelper.GetSystemAsset<EntityControlSubSystem>();
        componentSystem = AssetLoadHelper.GetSystemAsset<EntityComponentSystem>();
    }

    public override void OnUpdate(int deltaFrameCount, float deltaTime)
	{
		controlSubSystem.UpdateControl();
        componentSystem.OnUpdate(deltaFrameCount, deltaTime);

    }

	public IEnumerable<IEntity> GetDamagerEntities(int entityGuid, Vector3 pos, Vector3 size, Vector3 offset, Vector3 velocity)
	{
		return collisionSubSystem.GetDamagerEntities(entityGuid, pos, size, offset, velocity);
	}

	public IEnumerable<IEntity> GetAttackerEntities(int entityGuid, Vector3 pos, Vector3 size, Vector3 offset, Vector3 velocity)
	{
        return collisionSubSystem.GetAttackerEntities(entityGuid, pos, size, offset, velocity);
	}

	public IEnumerable<IEntity> GetOverlapEnemies(int entityGuid, Vector3 pos, Vector3 size, Vector3 offset, Vector3 velocity)
	{
        return collisionSubSystem.GetOverlapEnemies(entityGuid, pos, size, offset, velocity);
	}

	public void ToPlayerControl()
	{
		controlSubSystem.ToPlayerControl(Player);
	}

    public void ReleasePlayerControl()
    {
        controlSubSystem.ReleasePlayerControl();
    }

	public void ToAIControl()
	{
        foreach (var enemy in Enemies)
        {
			controlSubSystem.ToAIControl(enemy);
		}

        controlSubSystem.ToAIControl(Boss);
	}

    public void ReleaseAIControl()
    {
        controlSubSystem.ReleaseAIControl();
    }

	public void OnDrawGizmos()
	{
		collisionSubSystem.OnDrawGizmos();
	}

    public async UniTask<IEntity> CreateEnemy(ENUM_ENTITY_TYPE entityType)
    {
        return await CreateEntity<CharacterBehaviour>(entityType, false, false);
	}

    public async UniTask<IEntity> CreatePlayer(ENUM_ENTITY_TYPE entityType)
    {
        return await CreateEntity<CharacterBehaviour>(entityType, true, false);
    }

    public async UniTask<IEntity> CreateBoss(ENUM_ENTITY_TYPE entityType)
    {
        return await CreateEntity<CharacterBehaviour>(entityType, false, true);
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

		this.SetChildObject(entityBehaviour);

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
