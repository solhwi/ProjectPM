using StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterStateMachineComponent : MonoBehaviour
{
	private Animator animator;
	private CharacterAnimatorState[] animatorStates;

	[SerializeField] private EntityTransitionTable transitionTable = null;
	[SerializeField] private EntityConditionTable conditionTable = null;

	private void Reset()
	{
		transitionTable = AssetDatabase.LoadAssetAtPath<EntityTransitionTable>("Assets/Bundle/Datas/Parser/EntityTransitionTable.asset");
		conditionTable = AssetDatabase.LoadAssetAtPath<EntityConditionTable>("Assets/Bundle/Datas/Parser/EntityConditionTable.asset");
	}

	public ENUM_CHARACTER_STATE CurrentState
	{
		get;
		private set;
	} = ENUM_CHARACTER_STATE.Idle;

	public float CurrentNormalizedTime
	{
		get
		{
			return animator.GetCurrentNormalizedTime();
		}
	}

	public void Initialize(CharacterComponent owner)
	{
        CurrentState = ENUM_CHARACTER_STATE.Idle;

        animator = GetComponent<Animator>();
		animatorStates = animator.GetBehaviours<CharacterAnimatorState>();
		foreach (var state in animatorStates)
		{
			state.Initialize(owner);
		}
	}

	public bool ChangeState(ENUM_CHARACTER_STATE nextState, FrameCommandMessage stateMessage)
    {
		foreach (var state in animatorStates)
		{
			state.AddCommand(stateMessage);
		}

		bool isChangedState = CurrentState != nextState;
		if (isChangedState)
		{
			Debug.Log($"{Time.frameCount} 스테이트 변경 : {CurrentState} => {nextState}");
			animator.Play(nextState.ToString());
			CurrentState = nextState;
		}
		return isChangedState;
	}

	public ENUM_CHARACTER_STATE GetSimulatedNextState(FrameCommandMessage snapShotMessage, ENUM_CHARACTER_STATE currentState = ENUM_CHARACTER_STATE.None)
	{
		currentState = currentState == ENUM_CHARACTER_STATE.None ? CurrentState : currentState;

        foreach (var transition in transitionTable.transitionList)
		{
			if (transition.prevState == currentState)
			{
				var condition = conditionTable.GetCondition(transition.conditionType);
				if (condition.IsSatisfied(snapShotMessage))
				{
					return transition.nextState;
				}
			}
		}

		if (transitionTable.loopTransitionDictionary.TryGetValue(CurrentState, out var loopTransition))
		{
			var condition = conditionTable.GetCondition(loopTransition.conditionType);
			if (condition.IsSatisfied(snapShotMessage))
			{
				return currentState;
			}
			else
			{
				return ENUM_CHARACTER_STATE.Idle;
			}
		}
		else
		{
			var defaultTransition = transitionTable.defaultTransitionList.FirstOrDefault();
			var condition = conditionTable.GetCondition(defaultTransition.key);
			if (condition.IsSatisfied(snapShotMessage))
			{
				return defaultTransition.nextState;
			}
			else
			{
				return currentState;
			}
		}
	}
}
