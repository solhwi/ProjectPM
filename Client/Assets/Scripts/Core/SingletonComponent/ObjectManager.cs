using NPOI.HPSF;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ObjectManager : Singleton<ObjectManager>
{
    public ObjectComponent PlayerObject
    {
        get
        {
            if (objectDictionary.TryGetValue(playerObjectGuid, out var component))
            {
                return component;
            }

            Debug.LogError($"플레이어 오브젝트가 생성되기 전입니다. GUID : {playerObjectGuid}");
            return null;
        }
    }

    private int playerObjectGuid = 0;
    private Dictionary<int, ObjectComponent> objectDictionary = new Dictionary<int, ObjectComponent>();

    public IEnumerator LoadAsyncPlayer(ENUM_CHARACTER_TYPE characterType)
    {
        switch(characterType)
        {
            case ENUM_CHARACTER_TYPE.RedMan:
                yield return LoadAsyncPlayer<RedManComponent>();
                break;
        }
    }

    private IEnumerator LoadAsyncPlayer<T>() where T : CharacterComponent
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

        playerObjectGuid = character.Initialize(ENUM_TEAM_TYPE.Friendly, false);
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
        foreach(var obj in objectDictionary.Values)
        {
            obj.OnPostUpdate();
        }
	}

    public override void OnLateUpdate(int deltaFrameCount, float deltaTime)
    {
        foreach (var obj in objectDictionary.Values)
        {
            obj.OnLateUpdate();
        }
    }

    public int RegisterObject(int Guid, ObjectComponent objectComponent)
    {
        objectDictionary[Guid] = objectComponent;
        return Guid;
	}

    public int UnRegisterObject(int Guid)
    {
        if(objectDictionary.ContainsKey(Guid))
            objectDictionary.Remove(Guid);

        return -1;
    }
}
