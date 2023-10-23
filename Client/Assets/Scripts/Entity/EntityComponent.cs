using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 이 곳에 타입이 추가되면 Unity Layer에도 추가해야 합니다.
public enum ENUM_LAYER_TYPE
{
	Map = 3,
	Ground = 6,
	Platform = 7,
	Object = 8,
	Enemy = 9,
	Boss = 10,
	Friendly = 11,
	Projectile = 12,
	UI = 13,
}

public enum ENUM_TEAM_TYPE
{
	None = 0,
	Friendly = 1,
	Enemy = 2,
}

public enum ENUM_ENTITY_TYPE
{
	None = -1,
	RedMan = 0,
	BlueMan = 1,
	GreenMan = 2,
    PencilMan = 3,
}

// Entity를 데이터로 분리하지 않고,
// 베이스 클래스로 두어 상속받는다.

public abstract class EntityComponent : MonoBehaviour
{
	public ENUM_ENTITY_TYPE EntityType
	{
		get; private set;
	}

    public abstract int CurrentState
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

    public abstract Vector2 HitOffset
	{
		get;
	}

	public abstract bool IsLeftDirection
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

	public abstract float CurrentNormalizedTime
	{
		get;
	}

	public bool IsPlayer { get; private set; }

	public bool IsBoss => layerType == ENUM_LAYER_TYPE.Boss;

	public int OwnerGuid { get; private set; }

    public int EntityGuid { get; private set; }

	private ENUM_LAYER_TYPE layerType = ENUM_LAYER_TYPE.Object;


	public virtual void Initialize(int ownerGuid, int entityGuid, ENUM_ENTITY_TYPE type, bool isPlayer)
	{
		IsPlayer = isPlayer;
        OwnerGuid = ownerGuid;
		EntityType = type;
		EntityGuid = entityGuid;
	}

    public virtual void SetEntityLayer(ENUM_LAYER_TYPE layerType)
    {
		this.layerType = layerType;
		gameObject.layer = (int)layerType;
	}

    public void Clear()
	{
		IsPlayer = false;
        OwnerGuid = 0;
		EntityType = ENUM_ENTITY_TYPE.None;
		layerType = ENUM_LAYER_TYPE.Object;
		EntityGuid = 0;
	}

	public virtual void Teleport(Vector2 posVec)
	{
        SetPosition(posVec);
	}

	private void SetPosition(Vector2 posVec)
	{
        transform.position = posVec;
    }

    protected virtual int GetSimulatedNextState(ICommand command)
    {
        return CurrentState;
	}

	public virtual bool SendCommand(ICommand command)
	{
		return false;
	}

	public virtual void OnUpdate()
	{

	}
}
