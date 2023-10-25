using UnityEngine;
using UnityEngine.EventSystems;

public class Singleton
{
	protected SingletonBehaviour behaviour = null;
	protected static bool isReleased = false;

	protected Singleton()
	{

	}

	public void Initialize(SingletonBehaviour behaviour)
    {
		this.behaviour = behaviour;
        behaviour.RegisterSingleton(this);

        OnInitialize();
	}

	public void Release()
	{
		OnRelease();

		if(behaviour)
			behaviour.UnRegisterSingleton(this);
        
		isReleased = true;
	}

	protected virtual void OnInitialize()
	{

	}

	protected virtual void OnRelease()
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

// ���� �׳� �̱�����
// ScriptableObject�� �ٲ� �� ���� �� ��� �ʿ���
public abstract class Singleton<T> : Singleton where T : Singleton, new()
{
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
				instance = new T();
				instance.Initialize(SingletonBehaviour.Instance);
			}

			return instance;
		}
	}
}
