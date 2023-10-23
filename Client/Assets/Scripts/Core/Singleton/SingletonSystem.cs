using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SingletonSystem : MonoBehaviour
{
    public static SingletonSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SingletonSystem>();
                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }

    private static SingletonSystem instance;

    [System.Serializable] public class SingletonDictionary : SerializableDictionary<string, Singleton> { }
    [System.Serializable] public class SingletonObjectDictionary : SerializableDictionary<string, Transform> { }

    [SerializeField] private SingletonDictionary singletonDictionary = new SingletonDictionary();
    [SerializeField] private SingletonObjectDictionary singletonObjectDictionary = new SingletonObjectDictionary();

    public event Action onFixedUpdate;
    public event Action onUpdate;
    public event Action onLateUpdate;

    private WaitForEndOfFrame endOfFrameWait = new WaitForEndOfFrame();
    private Coroutine lateUpdateCoroutine = null;

    private void Awake()
    {
        lateUpdateCoroutine = StartCoroutine(OnLateUpdate());
    }

    private void OnDestroy()
    {
        if(lateUpdateCoroutine != null)
            StopCoroutine(lateUpdateCoroutine);
    }

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

		transform.SetPositionAndRotation(default, default);

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

    public void Destroy(MonoBehaviour m)
    {
        UnityEngine.Object.Destroy(m);
    }

    public void Destroy(GameObject go)
    {
		UnityEngine.Object.Destroy(go);
	}

	private void FixedUpdate()
	{
        onFixedUpdate?.Invoke();
	}

	private void Update()
    {
        onUpdate?.Invoke();
    }

    private IEnumerator OnLateUpdate()
    {
        while (true)
        {
            yield return endOfFrameWait;
            onLateUpdate?.Invoke();
        }
    }

    private void OnApplicationQuit()
    {
        foreach(var singleton in singletonDictionary.Values)
        {
            singleton?.Release();
        }
    }
}
