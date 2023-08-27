using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 물리 콜백도 이 곳에서 한다.
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

	private void OnCollisionEnter2D(Collision2D collision)
	{
		
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		
	}

	public void OnInput(FrameSyncInputData input)
	{
		if(characterComponent == null)
			return;

		characterComponent.Play(input);
	}
}
