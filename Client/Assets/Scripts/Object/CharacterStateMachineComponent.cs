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
		EntityAnimatorStateMachine.Initialize(animator, owner);
    }

    public void TryChangeState(ENUM_ENTITY_STATE nextState)
    {
		EntityAnimatorStateMachine.TryChangeState(animator, nextState);
	}
}
