using StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorComponent<TState> : MonoBehaviour where TState : Enum
{
	protected readonly int m_HashHorizontalSpeedParam = Animator.StringToHash("HorizontalSpeed");
	protected readonly int m_HashVerticalSpeedParam = Animator.StringToHash("VerticalSpeed");

	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	protected bool SetTrigger(TState state)
	{
		if (animator.IsInTransition(0))
			return false;

		animator.SetTrigger(state.ToString());
		return true;
	}

	protected bool ResetTrigger(TState state)
	{
		animator.ResetTrigger(state.ToString());
		return true;
	}

	protected bool SetBool(TState state, bool value)
	{
		if (animator.IsInTransition(0))
			return false;

		animator.SetBool(state.ToString(), value);
		return true;
	}

	protected bool SetInteger(TState state, int value)
	{
		if (animator.IsInTransition(0))
			return false;

		animator.SetInteger(state.ToString(), value);
		return true;
	}

	protected bool SetFloat(TState state, float value)
	{
		if (animator.IsInTransition(0))
			return false;

		animator.SetFloat(state.ToString(), value);
		return true;
	}

	public bool SetDirection(Vector2 move)
	{
		if (animator.IsInTransition(0))
			return false;

		animator.SetFloat(m_HashHorizontalSpeedParam, move.x);
		animator.SetFloat(m_HashVerticalSpeedParam, move.y);
		return true;
	}
}
