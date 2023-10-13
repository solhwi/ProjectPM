using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterController : Singleton<CharacterController>
{
    private PlayerInputComponent playerInputComponent = null;
    private Dictionary<int, AIInputComponent> aiInputComponentDictionary = new Dictionary<int, AIInputComponent>();
	
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

	public override void OnPrevUpdate(int deltaFrameCount, float deltaTime)
	{
        if(playerInputComponent)
            playerInputComponent.OnPrevUpdate(deltaFrameCount, deltaTime);

		foreach (var ai in aiInputComponentDictionary.Values)
        {
            ai.OnPrevUpdate(deltaFrameCount, deltaTime);
        }
	}

	public void RegisterAI(CharacterComponent character)
    {
        if (character == null)
            return;

        var runner = character.GetOrAddComponent<AIInputComponent>();
        if (runner == null) 
            return;

        runner.Run();
        aiInputComponentDictionary[character.Guid] = runner;
	}

    public void UnRegisterAI(CharacterComponent character)
	{
		if (character == null)
			return;

		if (aiInputComponentDictionary.ContainsKey(character.Guid) == false)
            return;

		aiInputComponentDictionary.Remove(character.Guid);
	}
}
