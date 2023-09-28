using NPOI.HPSF;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class EntityManager : Singleton<EntityManager>
{
    public EntityComponent Player
    {
        get
        {
            if (entityDictionary.TryGetValue(playerCharacterGuid, out var component))
            {
                return component;
            }

            Debug.LogError($"플레이어 오브젝트가 생성되기 전입니다. GUID : {playerCharacterGuid}");
            return null;
        }
    }

    private int playerCharacterGuid = 0;
    private Dictionary<int, EntityComponent> entityDictionary = new Dictionary<int, EntityComponent>();

    public IEnumerator LoadAsyncPlayer(ENUM_ENTITY_TYPE characterType)
    {
        var handle = ResourceManager.Instance.LoadAsync<EntityMeditatorComponent>();
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

        var character = obj.GetComponent<EntityMeditatorComponent>();
        if (character == null)
            yield break;

		character.Initialize(characterType);
		character.SetEntityLayer(ENUM_LAYER_TYPE.Friendly);

        playerCharacterGuid = character.Guid;

		mono.SetSingletonChild(this, character);
    }

    public IEnumerator LoadAsyncMonsters()
    {
        yield return null;
    }

    public IEnumerator LoadAsyncBoss()
    {
        yield return null;
    }

    public IEnumerator LoadAsyncPassiveObject()
    {
        yield return null;
    }

    public IEnumerator UnloadAsyncPlayer()
    {
        yield return null;
    }

    public IEnumerator UnloadAsyncMonsters()
    {
        yield return null;
    }

    public IEnumerator UnloadAsyncBoss()
    {
        yield return null;
    }

    public IEnumerator UnloadAsyncPassiveObject()
    {
        yield return null;
    }

	public override void OnPostUpdate(int deltaFrameCount, float deltaTime)
    {
        foreach(var obj in entityDictionary.Values)
        {
            obj.OnPostUpdate();
        }
	}

    public override void OnLateUpdate(int deltaFrameCount, float deltaTime)
    {
        foreach (var obj in entityDictionary.Values)
        {
            obj.OnLateUpdate();
        }
    }

    public int Register(EntityComponent objectComponent)
    {
        int Guid = objectComponent.GetInstanceID();
        entityDictionary[Guid] = objectComponent;
        return Guid;
	}

    public int UnRegister(int Guid)
    {
        if(entityDictionary.ContainsKey(Guid))
            entityDictionary.Remove(Guid);

        return -1;
    }
}
