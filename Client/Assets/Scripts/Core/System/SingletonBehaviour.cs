using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// �̱���� MonoBehaviour�� �������ִ� ����

public class SingletonBehaviour : MonoBehaviour
{
    public static SingletonBehaviour Instance
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject(typeof(SingletonBehaviour).ToString());
                instance = go.AddComponent<SingletonBehaviour>();
                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }

    private static SingletonBehaviour instance;

    [System.Serializable] public class SingletonDictionary : SerializableDictionary<string, Singleton> { }
    [SerializeField] private SingletonDictionary singletonDictionary = new SingletonDictionary();

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

    public void UnRegisterSingleton(Singleton singleton)
    {
        var type = singleton.GetType();
        var typeName = type.Name;

        if (singletonDictionary.ContainsKey(typeName))
            singletonDictionary.Remove(typeName);
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
    /// ������Ʈ �޽������� �����ǰ� �ʿ��Ͽ� ��� �ý��ۿ� �̰���
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
       
    }
}
