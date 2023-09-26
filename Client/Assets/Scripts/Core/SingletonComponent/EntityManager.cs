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
            if (characterDictionary.TryGetValue(playerCharacterGuid, out var component))
            {
                return component;
            }

            Debug.LogError($"플레이어 오브젝트가 생성되기 전입니다. GUID : {playerCharacterGuid}");
            return null;
        }
    }

    private int playerCharacterGuid = 0;
    private Dictionary<int, EntityComponent> characterDictionary = new Dictionary<int, EntityComponent>();

    public IEnumerator LoadAsyncPlayer(ENUM_ENTITY_TYPE characterType)
    {
        switch(characterType)
        {
            case ENUM_ENTITY_TYPE.RedMan:
                yield return LoadAsyncPlayer<RedManComponent>();
                break;
        }
    }

    private IEnumerator LoadAsyncPlayer<T>() where T : EntityMeditatorComponent
    {
        var handle = ResourceManager.Instance.LoadAsync<T>();
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

        T character = obj.GetComponent<T>();
        if (character == null)
            yield break;

		character.Initialize();
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
        foreach(var obj in characterDictionary.Values)
        {
            obj.OnPostUpdate();
        }
	}

    public override void OnLateUpdate(int deltaFrameCount, float deltaTime)
    {
        foreach (var obj in characterDictionary.Values)
        {
            obj.OnLateUpdate();
        }
    }

    public int Register(int Guid, EntityComponent objectComponent)
    {
        characterDictionary[Guid] = objectComponent;
        return Guid;
	}

    public int UnRegister(int Guid)
    {
        if(characterDictionary.ContainsKey(Guid))
            characterDictionary.Remove(Guid);

        return -1;
    }
}
