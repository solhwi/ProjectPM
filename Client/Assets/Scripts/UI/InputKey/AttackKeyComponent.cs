using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackKeyComponent : InputKeyComponent
{
    [SerializeField] private ENUM_ATTACK_KEY keyType = ENUM_ATTACK_KEY.MAX;

	public new event Action<ENUM_ATTACK_KEY, bool, int> onInputChanged = null;
	
	public override void OnDrag(PointerEventData eventData)
	{
		base.OnDrag(eventData);
		onInputChanged?.Invoke(keyType, isPressed, Time.frameCount);
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		onInputChanged?.Invoke(keyType, isPressed, Time.frameCount);
	}

	public override void OnClearPointerCallback()
	{
		base.OnClearPointerCallback();
		onInputChanged = null;
	}
}
