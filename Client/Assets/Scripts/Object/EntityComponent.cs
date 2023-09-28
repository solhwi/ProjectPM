using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityComponent : MonoBehaviour
{
	public ENUM_ENTITY_TYPE EntityType
	{
		get; private set;
	}

	public int Guid { get; private set; } = 0;

	public virtual void Initialize(ENUM_ENTITY_TYPE type)
	{
        EntityType = type;
        Guid = EntityManager.Instance.Register(this);
	}
	public void Clear()
	{
		EntityType = ENUM_ENTITY_TYPE.None;
        Guid = EntityManager.Instance.UnRegister(Guid);
	}

	public virtual void OnPostUpdate()
	{
		
	}

	public virtual void OnLateUpdate()
	{

	}
}
