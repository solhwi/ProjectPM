using StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimatorComponent : MonoBehaviour
{
    private CharacterComponent owner;
	private Animator animator;

    public void Initialize(CharacterComponent owner)
	{
		animator = GetComponent<Animator>();

		this.owner = owner;
        switch (owner)
        {
            case NormalCharacterComponent:
				CharacterLinkedSMB.Initialize(animator, owner);
                break;
		}
	}

    public void TryChangeState(FrameSyncCharacterInputData inputData)
    {
		switch (owner)
		{
			case NormalCharacterComponent:
				CharacterLinkedSMB.TryChangeState(inputData);
				break;
		}
	}
}
