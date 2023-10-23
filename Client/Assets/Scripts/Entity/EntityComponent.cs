using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// �� ���� Ÿ���� �߰��Ǹ� Unity Layer���� �߰��ؾ� �մϴ�.
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

// component�� entity�� ������� view�� �ݿ��Ѵ�.

// �̰� ��������� �ѵ�...
// 1. �׷� �浹�� ���� �κ��� ���� �����ؾ� �Ѵ�.
// 2. �ִϸ��̼ǵ� ���� �����ؼ� ������ ������ ������ �Ѵ�.

// �и��� ���ϱ�� ����...
// Entity �����͸� �и��ϴ� �� �ƴ϶� EntityComponent�� ��ӹ޴� �������� ����.
// Entity�� ����� ������ ������ �����Ѵ�.


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


	public virtual void Initialize(int ownerGuid, ENUM_ENTITY_TYPE type, bool isPlayer)
	{
		IsPlayer = isPlayer;
        OwnerGuid = ownerGuid;
		EntityType = type;
        EntityGuid = EntitySystem.Instance.Register(this);
	}

    public virtual void SetEntityLayer(ENUM_LAYER_TYPE layerType)
    {
		this.layerType = layerType;
	}

    public void Clear()
	{
		IsPlayer = false;
        OwnerGuid = 0;
		EntityType = ENUM_ENTITY_TYPE.None;
		layerType = ENUM_LAYER_TYPE.Object;
		EntityGuid = EntitySystem.Instance.UnRegister(EntityGuid);
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
