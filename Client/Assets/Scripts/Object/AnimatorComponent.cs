using StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorComponent<TState> : MonoBehaviour where TState : Enum
{
	protected Animator animator;

	protected virtual void Awake()
	{
		animator = GetComponent<Animator>();
	}

	public void Play(TState state, float normalizeTime = 0.0f)
	{
		animator.Play(state.ToString(), 0, normalizeTime);
	}
}
