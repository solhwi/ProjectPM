using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour, IInputReceiver
{
	public bool isEnable = true;

	CharacterComponent characterComponent = null;

	private void Awake()
	{
		characterComponent = GetComponent<CharacterComponent>();
	}

	private void OnEnable()
	{
		InputManager.Instance.RegisterInputReceiver(this);
	}

	private void OnDisable()
	{
		InputManager.Instance.UnregisterInputReceiver(this);
	}

	public void OnInput(FrameSyncInputData input)
	{
		if (!isEnable)
			return;

		if (characterComponent == null)
			return;

		characterComponent.OnInput(input);
	}
}
