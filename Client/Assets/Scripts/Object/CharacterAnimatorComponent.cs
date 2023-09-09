using StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterComponent))]
[RequireComponent(typeof(Animator))]
public class CharacterAnimatorComponent : MonoBehaviour
{
	private Animator animator;

    public void Initialize(CharacterComponent owner)
	{
		animator = GetComponent<Animator>();
        CharacterLinkedSMB.Initialize(animator, owner);
    }

    public void TryChangeState(FrameSyncCharacterStateInput inputData)
    {
		CharacterLinkedSMB.TryChangeState(inputData);
	}
}
