using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputComponent : MonoBehaviour
{
	protected virtual void OnEnable()
	{
		InputManager.Instance.Register(this);
	}

	protected virtual void OnDisable()
	{
		InputManager.Instance.UnRegister(this);
	}

	public virtual void OnUpdate(int deltaFrameCount, float deltaTime)
	{

	}
}

public class PlayerInputComponent : InputComponent
{
    private CharacterComponent characterComponent;

	protected override void OnEnable()
	{
		characterComponent = GetComponent<CharacterComponent>();
	}

	public override void OnUpdate(int deltaFrameCount, float deltaTime)
	{
		if (characterComponent == null)
			return;

		var command = MessageHelper.MakeCommand();
		characterComponent.TryChangeState(command);
	}
}
