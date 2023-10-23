using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GuardKeyComponent : InputKeyComponent
{
	public override void OnDrag(PointerEventData eventData)
	{
		base.OnDrag(eventData);
		FrameInputSystem.Instance.OnGuardInputChanged(isPressed, Time.frameCount);
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		FrameInputSystem.Instance.OnGuardInputChanged(isPressed, Time.frameCount);
	}
}
