using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class EntityComponent : MonoBehaviour
{
	public ENUM_ENTITY_TYPE EntityType
	{
		get; private set;
	}

    public abstract ENUM_ENTITY_STATE CurrentState
	{
		get;
	}

	public abstract Vector2 Velocity
	{
		get;
	}

	public abstract Vector2 HitBox
	{
		get;
	}

	public abstract Vector2 Offset
	{
		get;
	}

	public Vector2 Position
	{
		get
		{
			return transform.position;
		}
	}

	public abstract bool IsGrounded
	{
		get;
	}

	public int OwnerGuid { get; private set; }

    public int Guid { get; private set; } = 0;

	public virtual void Initialize(int ownerGuid, ENUM_ENTITY_TYPE type)
	{
		OwnerGuid = ownerGuid;
		EntityType = type;
        Guid = EntityManager.Instance.Register(this);
	}

	public void Clear()
	{
		OwnerGuid = 0;
		EntityType = ENUM_ENTITY_TYPE.None;
        Guid = EntityManager.Instance.UnRegister(Guid);
	}

	public virtual void Teleport(Vector2 posVec)
	{
		transform.position = posVec;
	}

	public virtual ENUM_ENTITY_STATE GetSimulatedNextState(IStateMessage stateInfo)
    {
        return CurrentState;
	}

	public virtual bool TryChangeState(IStateMessage stateInfo)
	{
		return false;
	}

    public virtual void OnPostUpdate()
	{
		
	}

	public virtual void OnLateUpdate()
	{

	}
}
