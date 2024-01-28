using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputKeyComponent : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	[SerializeField] protected FrameInputSystem inputSystem = null;
	protected bool isPressed = false;

    private void Reset()
    {
        inputSystem = AssetLoadHelper.GetSystemAsset<FrameInputSystem>();
    }

    public virtual void OnDrag(PointerEventData eventData)
	{
		isPressed = true;
	}

	public virtual void OnPointerDown(PointerEventData eventData)
	{
		OnDrag(eventData);
	}

	public virtual void OnPointerUp(PointerEventData eventData)
	{
		isPressed = false;
	}
}
