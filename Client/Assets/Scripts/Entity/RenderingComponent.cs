using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderingComponent : EntityComponent
{
	public bool IsLeftDirection
	{
		get;
		private set;
	}

	[SerializeField] private SpriteRenderer spriteRenderer = null;

	protected override void Reset()
	{
		base.Reset();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void Initialize(ENUM_LAYER_TYPE layerType, int orderIndex)
	{
		if (spriteRenderer == null)
			return;

		spriteRenderer.sortingOrder = LayerHelper.GetSortingLayer(layerType, orderIndex);
	}

	public void Look(bool isLeft)
	{
		if (spriteRenderer == null)
			return;

		spriteRenderer.flipX = isLeft;
		IsLeftDirection = isLeft;
    }
}
