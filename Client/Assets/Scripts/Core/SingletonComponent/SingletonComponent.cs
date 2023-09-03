using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SingletonComponent : MonoBehaviour, IUpdater
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


	public virtual void OnLateUpdate(int deltaFrameCount, float deltaTime)
	{

	}

	public virtual void OnPostUpdate(int deltaFrameCount, float deltaTime)
	{

	}

	public virtual void OnUpdate(int deltaFrameCount, float deltaTime)
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
				Debug.LogError($"{typeof(T)} 타입의 이미 해제된 싱글톤에 접근했습니다.");
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