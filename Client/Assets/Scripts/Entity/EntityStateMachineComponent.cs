using StateMachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EntityStateMachineComponent : MonoBehaviour
{
	private Animator animator;
	private EntityAnimatorState[] animatorStates;

	[SerializeField] private EntityTransitionTable transitionTable = null;
	[SerializeField] private EntityConditionTable conditionTable = null;

	private void Reset()
	{
		transitionTable = AssetDatabase.LoadAssetAtPath<EntityTransitionTable>("Assets/Bundle/Datas/Parser/EntityTransitionTable.asset");
		conditionTable = AssetDatabase.LoadAssetAtPath<EntityConditionTable>("Assets/Bundle/Datas/Parser/EntityConditionTable.asset");
	}

	public ENUM_ENTITY_STATE CurrentState
	{
		get;
		private set;
	} = ENUM_ENTITY_STATE.Idle;

	public float CurrentNormalizedTime
	{
		get
		{
			return animator.GetCurrentNormalizedTime();
		}
	}

	public int CurrentKeyFrame
	{
		get
		{
			return animator.GetCurrentKeyFrame();
		}
	}

	public void Initialize(EntityMeditatorComponent owner)
	{
        CurrentState = ENUM_ENTITY_STATE.Idle;

        animator = GetComponent<Animator>();
		animatorStates = animator.GetBehaviours<EntityAnimatorState>();
		foreach (var state in animatorStates)
		{
			state.Initialize(owner);
		}
	}

	public void ChangeState(ENUM_ENTITY_STATE nextState, FrameInputSnapShotMessage stateMessage)
    {
		foreach (var state in animatorStates)
		{
			state.SetStateMessage(stateMessage);
		}

		if (CurrentState != nextState)
		{
			Debug.Log($"스테이트 변경 {Time.frameCount} : {CurrentState} => {nextState}");
			animator.Play(nextState.ToString());
			CurrentState = nextState;
		}
	}

	public ENUM_ENTITY_STATE GetSimulatedNextState(FrameInputSnapShotMessage snapShotMessage, ENUM_ENTITY_STATE currentState = ENUM_ENTITY_STATE.None)
	{
		currentState = currentState == ENUM_ENTITY_STATE.None ? CurrentState : currentState;

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
				return ENUM_ENTITY_STATE.Idle;
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
