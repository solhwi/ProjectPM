using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityComponent : MonoBehaviour
{
	public abstract ENUM_ENTITY_TYPE EntityType
	{
		get;
	}

	public int Guid { get; private set; } = 0;

	public virtual void Initialize()
	{
		Guid = GetInstanceID();
		EntityManager.Instance.Register(Guid, this);
	}
	public void Clear()
	{
		EntityManager.Instance.UnRegister(Guid);
		Guid = 0;
	}

	public virtual void OnPostUpdate()
	{
		
	}

	public virtual void OnLateUpdate()
	{

	}
}
