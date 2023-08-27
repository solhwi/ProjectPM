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

public class ObjectComponent : MonoBehaviour
{
    [SerializeField] private ENUM_OBJECT_TYPE objectType = ENUM_OBJECT_TYPE.Object;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private new Collider2D collider;

    public ENUM_TEAM_TYPE TeamType { get; private set; } = ENUM_TEAM_TYPE.None;
    public bool IsBoss { get; private set; } = false;

    public virtual void Initialize()
    {

    }

    public virtual void Clear()
    {
        
    }

	public void SetOrder(int orderIndex, ENUM_TEAM_TYPE teamType = ENUM_TEAM_TYPE.None, bool isBoss = false)
    {
        if(objectType == ENUM_OBJECT_TYPE.Object && teamType == ENUM_TEAM_TYPE.Enemy)
        {
			objectType = ENUM_OBJECT_TYPE.Enemy;
		}
        else if(objectType == ENUM_OBJECT_TYPE.Object && teamType == ENUM_TEAM_TYPE.Friendly)
        {
			objectType = ENUM_OBJECT_TYPE.Friendly;
		}
        else if(objectType == ENUM_OBJECT_TYPE.Object && isBoss)
        {
            objectType = ENUM_OBJECT_TYPE.Boss;
        }

        TeamType = teamType;
		IsBoss = isBoss;

		gameObject.layer = (int)objectType;
		spriteRenderer.sortingOrder = LayerHelper.GetSortingLayer(objectType, orderIndex);
    }
}
