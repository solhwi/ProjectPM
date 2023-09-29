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

    public ENUM_ENTITY_STATE CurrentState
	{
		get; protected set;
	}

	public abstract Vector2 Velocity
	{
		get;
	}

	public abstract Vector2 HitBox
	{
		get;
	}

	public abstract bool IsGrounded
	{
		get;
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

    public virtual ENUM_ENTITY_STATE GetSimulatedNextState(IStateInfo stateInfo)
    {
        return CurrentState;
	}

	public virtual bool TryChangeState(IStateInfo stateInfo)
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
