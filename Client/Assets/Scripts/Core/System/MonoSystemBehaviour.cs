using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// MonoSystem과 MonoBehaviour를 연결해주는 역할

public class MonoSystemBehaviour : MonoBehaviour
{
    public static MonoSystemBehaviour Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MonoSystemBehaviour>();
                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }

    private static MonoSystemBehaviour instance;

    [System.Serializable] public class MonoSystemDictionary : SerializableDictionary<string, MonoSystem> { }
    [System.Serializable] public class MonoSystemObjectDictionary : SerializableDictionary<string, Transform> { }

    [SerializeField] private MonoSystemDictionary systemDictionary = new MonoSystemDictionary();
    [SerializeField] private MonoSystemObjectDictionary systemObjectDictionary = new MonoSystemObjectDictionary();

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

    public void RegisterSystem(MonoSystem singleton)
    {
        var type = singleton.GetType();
        var typeName = type.Name;

        systemDictionary[typeName] = singleton;
    }

    public void UnRegisterSystem(MonoSystem singleton)
    {
        var type = singleton.GetType();
        var typeName = type.Name;

        if (systemDictionary.ContainsKey(typeName))
            systemDictionary.Remove(typeName);
    }

    private Transform RequestSystemGameObject(MonoSystem singleton)
    {
        var type = singleton.GetType();
        var typeName = type.Name;

        var g = new GameObject(typeName);
        var tr = g.transform;

		transform.SetPositionAndRotation(default, default);

		tr.SetParent(transform);
        tr.SetPositionAndRotation(default, default);
        tr.SetAsLastSibling();

        systemObjectDictionary.Add(typeName, tr);
        return tr;
    }

    public void SetSystemChild(MonoSystem singleton, MonoBehaviour childObj)
    {
        var type = singleton.GetType();
        var typeName = type.Name;

        if (systemObjectDictionary.TryGetValue(typeName, out var singletonTransform) == false)
        {
            singletonTransform = RequestSystemGameObject(singleton);
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

    /// <summary>
    /// 업데이트 메시지들은 재정의가 필요하여 모듈 시스템에 이관함
    /// </summary>

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
        foreach(var singleton in systemDictionary.Values)
        {
            singleton?.Release();
        }
    }
}
