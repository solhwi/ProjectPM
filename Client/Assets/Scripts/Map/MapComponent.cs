using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum ENUM_MAP_TYPE
{
    City = 0,
}

public abstract class MapComponent : MonoBehaviour
{
    public abstract ENUM_MAP_TYPE MapType
    {
        get;
    }

    public void SetOrderLayer()
    {
		gameObject.layer = (int)ENUM_LAYER_TYPE.Map;

		int order = 0;
        foreach(var renderer in GetComponentsInChildren<SpriteRenderer>())
        {
			renderer.gameObject.layer = (int)ENUM_LAYER_TYPE.Map;
			renderer.sortingOrder = LayerHelper.GetSortingLayer(ENUM_LAYER_TYPE.Map, order++);
        }

		foreach (var groundComponent in GetComponentsInChildren<GroundComponent>())
		{
			groundComponent.gameObject.layer = (int)ENUM_LAYER_TYPE.Ground;
		}

	}
}
