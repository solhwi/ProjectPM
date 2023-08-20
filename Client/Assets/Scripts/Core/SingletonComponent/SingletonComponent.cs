using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SingletonComponent : MonoBehaviour
{
	public void Initialize()
	{
		DontDestroyOnLoad(gameObject);
		OnAwakeInstance();
	}

	public void Release()
	{
		OnReleaseInstance();
	}

	protected virtual void OnAwakeInstance()
	{
		
	}

	protected virtual void OnReleaseInstance()
	{
		
	}
}

public class SingletonComponent<T> : SingletonComponent where T : SingletonComponent
{
	private static bool isReleased = false;

	private static T instance;
	public static T Instance
	{
		get
		{
			if (isReleased)
			{
				Debug.LogError($"{typeof(T)} Ÿ���� �̹� ������ �̱��濡 �����߽��ϴ�.");
				return null;
			}

			if (instance == null)
			{
				var g = new GameObject(typeof(T).Name);
                instance = g.AddComponent<T>();
                instance.Initialize();
			}

			return instance;
		}
	}

	private void OnApplicationQuit()
	{
		isReleased = true;
		Release();
	}
}