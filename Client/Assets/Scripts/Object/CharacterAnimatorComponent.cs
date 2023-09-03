using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;
using System;
using System.ComponentModel;

public class CharacterAnimatorComponent : AnimatorComponent<CharacterState>
{
	protected CharacterComponent component = null;

	protected override void Awake()
	{
        base.Awake();

        component = GetComponent<CharacterComponent>();
        CharacterLinkedSMB.Initialize(animator, this);
	}

	public void TryChangeState(FrameSyncCharacterInputData inputData)
	{
        CharacterLinkedSMB.TryChangeState(inputData);
    }

	public void OnPostStateUpdate(FrameSyncCharacterOutputData outputData)
	{
		if (component == null)
			return;

		component.OnPostStateUpdate(outputData);
    }
}
