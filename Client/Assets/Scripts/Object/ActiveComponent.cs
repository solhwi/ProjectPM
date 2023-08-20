using StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorComponent<TState> : MonoBehaviour where TState : Enum
{
	[SerializeField] protected Animator animator;

	public bool SetTrigger(TState state)
	{
		if (animator.IsInTransition(0))
			return false;

		animator.SetTrigger(state.ToString());
		return true;
	}

	public bool ResetTrigger(TState state)
	{
		animator.ResetTrigger(state.ToString());
		return true;
	}

	public bool SetBool(TState state, bool value)
	{
		if (animator.IsInTransition(0))
			return false;

		animator.SetBool(state.ToString(), value);
		return true;
	}

	public bool SetInteger(TState state, int value)
	{
		if (animator.IsInTransition(0))
			return false;

		animator.SetInteger(state.ToString(), value);
		return true;
	}

	public bool SetFloat(TState state, float value)
	{
		if (animator.IsInTransition(0))
			return false;

		animator.SetFloat(state.ToString(), value);
		return true;
	}
}
