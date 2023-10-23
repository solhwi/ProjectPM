using Cysharp.Threading.Tasks;
using NPOI.SS.Formula.Functions;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class EntitySystem : Singleton<EntitySystem>
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
    
    public IEnumerable<CharacterComponent> Enemies
    {
        get
        {
            return entityDictionary.Values.Where(e => e.IsPlayer == false).OfType<CharacterComponent>();
        }
    }

    private Dictionary<int, EntityComponent> entityDictionary = new Dictionary<int, EntityComponent>();

	public override void OnUpdate(int deltaFrameCount, float deltaTime)
	{
        foreach(var entity in entityDictionary.Values)
        {
            entity.OnUpdate();
        }
	}

    public CharacterComponent GetEnemy(ENUM_ENTITY_TYPE entityType)
    {
		return Enemies.FirstOrDefault(e => e.IsBoss == false && e.EntityType == entityType);  
	}

    public async UniTask<IEnumerable<EntityComponent>> LoadAsyncEnemies(IEnumerable<EnemySpawnData> spawnDatas)
    {
        var entities = new List<EntityComponent>();

        foreach(var data in spawnDatas)
        {
            entities.Add(await LoadAsyncEnemy(data.entityType));
		}

        return entities;
    }

    public async UniTask<EntityComponent> LoadAsyncEnemy(ENUM_ENTITY_TYPE entityType)
    {
        return await CreateEntity(entityType, false, false);
	}

    public async UniTask<EntityComponent> LoadAsyncPlayer(ENUM_ENTITY_TYPE entityType)
    {
        return await CreateEntity(entityType, true, false);
    }

    public async UniTask<EntityComponent> LoadAsyncBoss(EnemySpawnData spawnData)
    {
        return await CreateEntity(spawnData.entityType, false, true);
    }

    private async UniTask<EntityComponent> CreateEntity(ENUM_ENTITY_TYPE characterType, bool isPlayer, bool isBoss)
    {
        var character = await AddressableResourceSystem.Instance.InstantiateAsync<CharacterComponent>();

#if UNITY_EDITOR
        character.name = characterType.ToString();
#endif

        int ownerGuid = GameConfig.MonsterGuid;
        ENUM_LAYER_TYPE layerType = ENUM_LAYER_TYPE.Enemy;

        if (isPlayer)
        {
            ownerGuid = GameConfig.PlayerGuid;
            layerType = ENUM_LAYER_TYPE.Friendly;

            PlayerCharacter = character;
        }
        else if(isBoss)
        {
            layerType = ENUM_LAYER_TYPE.Boss;
            BossCharacter = character;
        }

		int entityGuid = character.GetInstanceID();

		character.Initialize(ownerGuid, entityGuid, characterType, isPlayer);
        character.SetEntityLayer(layerType);
        mono.SetSingletonChild(this, character);

		entityDictionary[entityGuid] = character;
		return character;
    }

    public EntityComponent GetEntityComponent(int guid)
    {
        if (entityDictionary.ContainsKey(guid) == false)
            return null;

        return entityDictionary[guid];
    }

    public CharacterComponent GetCharacterComponent(int guid)
    {
        return GetEntityComponent(guid) as CharacterComponent;
    }

    public IEnumerable<EntityComponent> GetAllEntities()
    {
        return entityDictionary.Values;
    }

    public IEnumerable<EntityComponent> GetEntities(int ownerGuid)
    {
        return GetAllEntities().Where(e => e.OwnerGuid == ownerGuid);
    }
}
