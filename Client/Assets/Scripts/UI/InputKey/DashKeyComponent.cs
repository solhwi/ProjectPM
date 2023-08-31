using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DashKeyComponent : InputKeyComponent
{
	public override void OnDrag(PointerEventData eventData)
	{
		base.OnDrag(eventData);
		InputManager.Instance.OnDashInputChanged(isPressed, Time.frameCount);
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		InputManager.Instance.OnDashInputChanged(isPressed, Time.frameCount);
	}
}
