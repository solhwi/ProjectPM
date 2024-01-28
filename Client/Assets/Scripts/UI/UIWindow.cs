using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
public class UIWindow : MonoBehaviour
{
	[SerializeField] protected PrefabLinkedUISystem uiSystem = null;
    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasScaler scaler;

    private void Reset()
	{
        uiSystem = AssetLoadHelper.GetSystemAsset<PrefabLinkedUISystem>();
        canvas = GetComponent<Canvas>();
		scaler = GetComponent<CanvasScaler>();

		SetCanvas();
		SetScaler();
		SetOrder(0);
	}

	private void Awake()
	{
		canvas.worldCamera = Camera.main;
	}

	private void SetCanvas()
	{
		canvas.renderMode = RenderMode.ScreenSpaceCamera;
	}

	private void SetScaler()
	{
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(1920, 1080);
		scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
		scaler.referencePixelsPerUnit = 100;
	}

	public void SetOrder(int orderIndex)
	{
		canvas.sortingOrder = LayerHelper.GetSortingLayer(gameObject, orderIndex);
	}
}
