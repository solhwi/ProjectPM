using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MonoBehaviourManager : MonoBehaviour
{
    public static MonoBehaviourManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MonoBehaviourManager>();
                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }

    private static MonoBehaviourManager instance;

    [System.Serializable] public class SingletonDictionary : SerializableDictionary<string, Singleton> { }
    [System.Serializable] public class SingletonObjectDictionary : SerializableDictionary<string, Transform> { }

    [SerializeField] private SingletonDictionary singletonDictionary = new SingletonDictionary();
    [SerializeField] private SingletonObjectDictionary singletonObjectDictionary = new SingletonObjectDictionary();

    public void RegisterSingleton(Singleton singleton)
    {
        var type = singleton.GetType();
        var typeName = type.Name;

        singletonDictionary[typeName] = singleton;
    }

    public void UnRegisterSinglton(Singleton singleton)
    {
        var type = singleton.GetType();
        var typeName = type.Name;

        if (singletonDictionary.ContainsKey(typeName))
            singletonDictionary.Remove(typeName);
    }

    private Transform RequestSingletonGameObject(Singleton singleton)
    {
        var type = singleton.GetType();
        var typeName = type.Name;

        var g = new GameObject(typeName);
        var tr = g.transform;

        tr.SetParent(transform);
        tr.SetPositionAndRotation(default, default);
        tr.SetAsLastSibling();

        singletonObjectDictionary.Add(typeName, tr);
        return tr;
    }

    public void SetSingletonChild(Singleton singleton, MonoBehaviour childObj)
    {
        var type = singleton.GetType();
        var typeName = type.Name;

        if (singletonObjectDictionary.TryGetValue(typeName, out var singletonTransform) == false)
        {
            singletonTransform = RequestSingletonGameObject(singleton);
        }

        childObj.transform.SetParent(singletonTransform);
        childObj.transform.SetPositionAndRotation(default, default);
        childObj.transform.SetAsLastSibling();
    }

    public new Coroutine StartCoroutine(IEnumerator routine)
    {
        return base.StartCoroutine(routine);
    }

    public new void StopCoroutine(Coroutine routine)
    {
        base.StopCoroutine(routine);
    }

    private void OnApplicationQuit()
    {
        foreach(var singleton in singletonDictionary.Values)
        {
            singleton.Release();
        }
    }
}
