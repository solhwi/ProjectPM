using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderingComponent : MonoBehaviour
{
	public void Initialize(ENUM_LAYER_TYPE layerType, int orderIndex)
	{
		var spriteRenderer = GetComponent<SpriteRenderer>();
		if (spriteRenderer == null)
			return;

		spriteRenderer.sortingOrder = LayerHelper.GetSortingLayer(layerType, orderIndex);
	}
}
