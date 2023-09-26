using StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterStateMachineComponent : MonoBehaviour
{
	private Animator animator;

    public void Initialize(EntityMeditatorComponent owner)
	{
		animator = GetComponent<Animator>();
		CharacterAnimatorStateMachine.Initialize(animator, owner);
    }

    public void TryChangeState(FrameSyncStateParam stateParam)
    {
		CharacterAnimatorStateMachine.TryChangeState(stateParam);
	}
}
