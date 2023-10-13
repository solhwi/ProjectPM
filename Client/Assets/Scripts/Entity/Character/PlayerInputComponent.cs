using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputComponent : MonoBehaviour
{
	private void OnEnable()
	{
		InputManager.Instance.Register(this);
	}

	private void OnDisable()
	{
		InputManager.Instance.UnRegister(this);
	}

	public virtual void OnUpdate(int deltaFrameCount, float deltaTime)
	{

	}
}

public class PlayerInputComponent : InputComponent
{
	private bool isEnable = false;
    private CharacterComponent characterComponent;

	public void Run()
	{
		if(characterComponent == null)
			characterComponent = GetComponent<CharacterComponent>();

		isEnable = true;
	}

	public void Stop()
	{
		characterComponent = null;
		isEnable = false;
	}

	public override void OnUpdate(int deltaFrameCount, float deltaTime)
	{
		if (isEnable == false)
			return;

		if (characterComponent == null)
			return;

		var command = MessageHelper.MakeCommand();
		characterComponent.TryChangeState(command);
	}
}
