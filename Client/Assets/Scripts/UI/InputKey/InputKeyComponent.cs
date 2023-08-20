using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputKeyComponent : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	protected bool isPressed = false; 
	public event Action<bool, int> onInputChanged = null;

	public virtual void OnClearPointerCallback()
	{
		onInputChanged = null;
	}

	public virtual void OnDrag(PointerEventData eventData)
	{
		isPressed = true;
		onInputChanged?.Invoke(isPressed, Time.frameCount);
	}

	public virtual void OnPointerDown(PointerEventData eventData)
	{
		OnDrag(eventData);
	}

	public virtual void OnPointerUp(PointerEventData eventData)
	{
		isPressed = false;
		onInputChanged?.Invoke(isPressed, Time.frameCount);
	}
}
