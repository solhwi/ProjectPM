using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputComponent : MonoBehaviour
{
	protected bool isEnable = false;

	public virtual void Run()
	{
		isEnable = true;
	}

	public virtual void Stop()
	{
		isEnable = false;
	}

	public virtual void OnPrevUpdate(int deltaFrameCount, float deltaTime)
	{

	}
}

public class PlayerInputComponent : InputComponent
{
    private CharacterComponent characterComponent;

	public override void Run()
	{
		base.Run();

		if(characterComponent == null)
			characterComponent = GetComponent<CharacterComponent>();
	}

	public override void Stop()
	{
		base.Stop();

		characterComponent = null;
	}

	public override void OnPrevUpdate(int deltaFrameCount, float deltaTime)
	{
		if (isEnable == false)
			return;

		if (characterComponent == null)
			return;

		var command = MessageHelper.MakeCommand();
		characterComponent.TryChangeState(command);
	}
}
