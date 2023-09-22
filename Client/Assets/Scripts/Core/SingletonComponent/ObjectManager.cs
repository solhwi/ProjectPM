using NPOI.HPSF;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ObjectManager : SingletonComponent<ObjectManager>
{
    public ObjectComponent PlayerObject
    {
        get
        {
            if (objectDictionary.TryGetValue(playerObjectGuid, out var component))
            {
                return component;
            }

            Debug.LogError($"�÷��̾� ������Ʈ�� �����Ǳ� ���Դϴ�. GUID : {playerObjectGuid}");
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

        var obj = Instantiate(prefab);
        if (obj == null)
            yield break;

        T character = obj.GetComponent<T>();
        if (character == null)
            yield break;

        character.Initialize(ENUM_TEAM_TYPE.Friendly, false);

        character.transform.SetParent(transform);
        character.transform.SetPositionAndRotation(default, default);
        character.transform.SetAsLastSibling();

        character.AddComponent<PlayerInput>();

        playerObjectGuid = character.Guid;
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
            obj.OnPostInput();
        }
	}

    public override void OnLateUpdate(int deltaFrameCount, float deltaTime)
    {
        foreach (var obj in objectDictionary.Values)
        {
            obj.OnUpdateAnimation();
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
