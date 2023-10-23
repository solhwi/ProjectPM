using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Singleton : IUpdater
{
	protected SingletonSystem mono = null;
    protected static bool isReleased = false;

    protected Singleton()
	{
        
    }

    public void Initialize(SingletonSystem mono)
	{
        this.mono = mono;
        mono.RegisterSingleton(this);

        OnAwakeInstance();
	}

	public void Release()
	{
		mono.UnRegisterSinglton(this);

		OnReleaseInstance();
        isReleased = true;
    }

    protected virtual void OnAwakeInstance()
	{
		
	}

	protected virtual void OnReleaseInstance()
	{
		
	}

	public virtual void OnFixedUpdate(int deltaFrameCount, float deltaTime)
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

	public virtual void OnPrevUpdate(int deltaFrameCount, float deltaTime)
	{

	}
}

public class Singleton<T> : Singleton where T : Singleton, new()
{
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
                instance = new T();
                instance.Initialize(SingletonSystem.Instance);
			}

			return instance;
		}
	}
}