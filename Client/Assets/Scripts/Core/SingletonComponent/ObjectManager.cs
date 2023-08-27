using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ObjectManager : SingletonComponent<ObjectManager>
{
    public CharacterComponent PlayerCharacter
    {
        get;
        private set;
    }

    public IEnumerator LoadAsyncPlayer(ENUM_CHARACTER_TYPE characterType)
    {
        switch(characterType)
        {
            case ENUM_CHARACTER_TYPE.Normal:
                yield return LoadAsyncPlayer<NormalCharacterComponent>();
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

        PlayerCharacter = character;
        PlayerCharacter.Initialize();
        PlayerCharacter.SetOrder(0, ENUM_TEAM_TYPE.Friendly);

        PlayerCharacter.transform.SetParent(transform);
        PlayerCharacter.transform.SetPositionAndRotation(default, default);
        PlayerCharacter.transform.SetAsLastSibling();
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
}
