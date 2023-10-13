using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterController : Singleton<CharacterController>
{
    private PlayerInputComponent playerInputComponent = null;

	public void RegisterControl(CharacterComponent character)
    {
		playerInputComponent = character.GetOrAddComponent<PlayerInputComponent>();
        if (playerInputComponent == null)
            return;

		playerInputComponent.Run();
	}

    public void UnRegisterControl()
    {
        playerInputComponent.Stop();
    }
}
