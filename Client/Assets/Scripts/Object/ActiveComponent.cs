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

	protected Animator animator;

	protected virtual void Awake()
	{
		animator = GetComponent<Animator>();
	}

	protected bool SetTrigger(TState state)
	{
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
		animator.SetBool(state.ToString(), value);
		return true;
	}

	protected bool SetInteger(TState state, int value)
	{
		animator.SetInteger(state.ToString(), value);
		return true;
	}

	protected bool SetFloat(TState state, float value)
	{
		animator.SetFloat(state.ToString(), value);
		return true;
	}

	public bool SetDirection(Vector2 move)
	{
		animator.SetFloat(m_HashHorizontalSpeedParam, move.x);
		animator.SetFloat(m_HashVerticalSpeedParam, move.y);
		return true;
	}

	protected bool CanTransition()
	{
		return true;

		// ���� ������Ʈ�� has exit time�� ������ �ְ�, ���� Ŭ���� ������� �ʾҴٸ� �Ұ���
		// ���� Ʈ������ ���̶�� �Ұ���
	}
}
