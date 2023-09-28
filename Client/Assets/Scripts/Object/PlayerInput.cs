using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour, IInputReceiver
{
	public bool isEnable = true;

	EntityMeditatorComponent characterComponent = null;

	private void Awake()
	{
		characterComponent = GetComponent<EntityMeditatorComponent>();
	}

	private void OnEnable()
	{
		InputManager.Instance.RegisterInputReceiver(this);
	}

	private void OnDisable()
	{
		InputManager.Instance.UnregisterInputReceiver(this);
	}

	public void OnInput(FrameSyncInputMessage input)
	{
		if (!isEnable)
			return;

		if (characterComponent == null)
			return;

		characterComponent.OnPlayerInput(input);
	}
}
