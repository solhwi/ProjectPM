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

// ���Ǹ� ���� Entity�� ������ ������ �и����� �ʰ�,

// �������̽� Ŭ������ �ξ� ����� �����ϰ�,
// MonoBehaviour�� ��ӹ��� EntityBehaviour�� �����Ѵ�.

public abstract class EntityBehaviour : MonoBehaviour, IEntity
{
	public virtual int CurrentState => 0;

	public virtual bool IsAttackable => false;

	public virtual Vector2 Velocity => Vector2.zero;

	public virtual Vector2 HitBox => Vector2.zero;

	public virtual Vector2 HitOffset => Vector2.zero;

	public virtual bool IsLeftDirection => false;

	public virtual Vector2 Position => transform.position;

	public virtual bool IsGrounded => true;

	public virtual float CurrentStateNormalizedTime => 0.0f;

	public ENUM_ENTITY_TYPE EntityType => entityType;

	public ENUM_TEAM_TYPE TeamType
	{
		get
		{
			if (layerType == ENUM_LAYER_TYPE.Friendly)
				return ENUM_TEAM_TYPE.Friendly;

			else if(layerType == ENUM_LAYER_TYPE.Enemy || layerType == ENUM_LAYER_TYPE.Boss)
				return ENUM_TEAM_TYPE.Enemy;

			return ENUM_TEAM_TYPE.None;
		}
	}

	public bool IsPlayer => isPlayer;

	public bool IsBoss => layerType == ENUM_LAYER_TYPE.Boss;

	public int OwnerGuid => ownerGuid;

	public int EntityGuid => entityGuid;

	private bool isPlayer = false;
	private int ownerGuid = 0;
	private int entityGuid = 0;
	private ENUM_ENTITY_TYPE entityType = ENUM_ENTITY_TYPE.None;
	private ENUM_LAYER_TYPE layerType = ENUM_LAYER_TYPE.Object;

	public virtual void Initialize(int ownerGuid, int entityGuid, ENUM_ENTITY_TYPE type, bool isPlayer)
	{
		this.isPlayer = isPlayer;
		this.ownerGuid = ownerGuid;
		this.entityType = type;
		this.entityGuid = entityGuid;
	}

    public virtual void SetEntityLayer(ENUM_LAYER_TYPE layerType)
    {
		this.layerType = layerType;
		gameObject.layer = (int)layerType;
	}

    public void Clear()
	{
		this.isPlayer = false;
        this.ownerGuid = 0;
		this.entityType = ENUM_ENTITY_TYPE.None;
		this.layerType = ENUM_LAYER_TYPE.Object;
		this.entityGuid = 0;
	}

	public virtual bool IsDamageable(IEntity attackerEntity)
	{
		return attackerEntity.IsAttackable;
	}

	public void SetPosition(Vector2 position)
	{
		transform.position = position;
	}

	public virtual void PushCommand(ICommand command)
	{
		
	}
}
