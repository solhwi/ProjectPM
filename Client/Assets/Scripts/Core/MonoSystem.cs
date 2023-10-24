using UnityEngine;

public class MonoSystem
{
	protected MonoSystemBehaviour behaviour = null;
	protected static bool isReleased = false;

	protected MonoSystem()
	{

	}

	public void Initialize(MonoSystemBehaviour mono)
	{
		this.behaviour = mono;
		mono.RegisterSystem(this);

		OnInitializeSystem();
	}

	public void Release()
	{
		behaviour.UnRegisterSystem(this);

		OnReleaseSystem();
		isReleased = true;
	}

	protected virtual void OnInitializeSystem()
	{

	}

	protected virtual void OnReleaseSystem()
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
public abstract class MonoSystem<T> : MonoSystem where T : MonoSystem, new()
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
				instance.Initialize(MonoSystemBehaviour.Instance);
			}

			return instance;
		}
	}
}
