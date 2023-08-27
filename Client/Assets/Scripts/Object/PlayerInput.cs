using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� �ݹ鵵 �� ������ �Ѵ�.
public class PlayerInput : MonoBehaviour, IInputReceiver
{
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
		if(characterComponent == null)
			return;

		characterComponent.Play(input);
	}
}
