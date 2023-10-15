using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderingComponent : MonoBehaviour
{
	public bool IsLeftDirection
	{
		get;
		private set;
	}

	private SpriteRenderer spriteRenderer = null;

    public void Initialize(ENUM_LAYER_TYPE layerType, int orderIndex)
	{
        spriteRenderer = GetComponent<SpriteRenderer>();
		if (spriteRenderer == null)
			return;

		spriteRenderer.sortingOrder = LayerHelper.GetSortingLayer(layerType, orderIndex);
	}

	public void Look(bool isLeft)
	{
        spriteRenderer.flipX = isLeft;
		IsLeftDirection = isLeft;
    }
}
