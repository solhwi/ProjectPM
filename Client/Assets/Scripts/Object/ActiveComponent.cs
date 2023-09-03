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

		// 현재 스테이트가 has exit time을 가지고 있고, 아직 클립이 종료되지 않았다면 불가능
		// 현재 트랜지션 중이라면 불가능
	}
}
