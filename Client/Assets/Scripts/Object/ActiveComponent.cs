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

	public void Play(TState state, float normalizeTime = 0.0f)
	{
		animator.Play(state.ToString(), 0, normalizeTime);
	}

	protected virtual bool CanTransition()
	{
		return true;

		// ���� ������Ʈ�� has exit time�� ������ �ְ�, ���� Ŭ���� ������� �ʾҴٸ� �Ұ���
		// ���� Ʈ������ ���̶�� �Ұ���
	}
}
