using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이 곳에 타입이 추가되면 Unity Layer에도 추가해야 합니다.
public enum ENUM_OBJECT_TYPE
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
    None = -1,
    Friendly = 0,
    Enemy = 1,
}

[RequireComponent(typeof(SpriteRenderer))]
public abstract class ObjectComponent : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

	public ENUM_OBJECT_TYPE ObjectType { get; private set; } = ENUM_OBJECT_TYPE.Object;

	public ENUM_TEAM_TYPE TeamType { get; private set; } = ENUM_TEAM_TYPE.None;
    public bool IsBoss { get; private set; } = false;

    public int Guid { get; private set; } = 0;

    public virtual void Initialize(ENUM_TEAM_TYPE teamType, bool isBoss)
	{
		TeamType = teamType;
		IsBoss = isBoss;

		if (isBoss)
		{
			ObjectType = ENUM_OBJECT_TYPE.Boss;
		}
		else if (teamType == ENUM_TEAM_TYPE.Enemy)
		{
			ObjectType = ENUM_OBJECT_TYPE.Enemy;
		}
		else if (teamType == ENUM_TEAM_TYPE.Friendly)
		{
			ObjectType = ENUM_OBJECT_TYPE.Friendly;
		}

		gameObject.layer = (int)ObjectType;

        Guid = GetInstanceID();
		int orderIndex = ObjectManager.Instance.RegisterObject(Guid, this);

		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sortingOrder = LayerHelper.GetSortingLayer(ObjectType, orderIndex);
    }

    public virtual void Clear()
    {
        spriteRenderer = null; 

		ObjectManager.Instance.UnRegisterObject(Guid);
		Guid = 0;

	}

	public virtual void OnOtherInput(IEnumerable<AttackableComponent> attackers)
	{
		
	}

    public virtual void OnPostInput()
    {

    }

    public virtual void OnUpdateAnimation()
    {

    }
}
