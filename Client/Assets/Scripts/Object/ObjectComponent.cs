using System.Collections;
using System.Collections.Generic;
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

// 총알 등 때문에 ObjectComponent에 둠
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

	public ENUM_TEAM_TYPE TeamType { get; private set; } = ENUM_TEAM_TYPE.None;
	public ENUM_LAYER_TYPE LayerType { get; private set; } = ENUM_LAYER_TYPE.Object;

    public int Guid { get; private set; } = 0;

    public virtual void Initialize(ENUM_TEAM_TYPE teamType, bool isBoss)
	{
		Guid = GetInstanceID();
		TeamType = teamType;

		int orderIndex = ObjectManager.Instance.RegisterObject(Guid, this);

		if (isBoss)
		{
			LayerType = ENUM_LAYER_TYPE.Boss;
		}
		else if (teamType == ENUM_TEAM_TYPE.Enemy)
		{
			LayerType = ENUM_LAYER_TYPE.Enemy;
		}
		else if (teamType == ENUM_TEAM_TYPE.Friendly)
		{
			LayerType = ENUM_LAYER_TYPE.Friendly;
		}

		gameObject.layer = (int)LayerType;

		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sortingOrder = LayerHelper.GetSortingLayer(LayerType, orderIndex);
    }

    public virtual void Clear()
    {
        spriteRenderer = null;
		Guid = ObjectManager.Instance.UnRegisterObject(Guid);
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
