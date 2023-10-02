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
		animator = GetComponent<Animator>();
		animatorStates = animator.GetBehaviours<EntityAnimatorState>();
		foreach (var state in animatorStates)
		{
			state.Initialize(owner);
		}
		CurrentState = ENUM_ENTITY_STATE.Idle;
	}

	public void ChangeState(ENUM_ENTITY_STATE nextState)
	{
		ChangeState(nextState, new NoStateMessage());
	}

	public void ChangeState(ENUM_ENTITY_STATE nextState, IStateMessage stateMessage)
    {
		foreach (var state in animatorStates)
		{
			state.SetStateMessage(stateMessage);
		}

		Debug.Log($"스테이트 변경 : {CurrentState} => {nextState}");
		animator.Play(nextState.ToString());
		CurrentState = nextState;
	}

	public ENUM_ENTITY_STATE GetSimulatedNextState(IStateMessage stateInfo)
	{
		var nextState = CurrentState;

		if (transitionTable.defaultTransitionList.Any())
		{
			var defaultTransition = transitionTable.defaultTransitionList.FirstOrDefault();
			var condition = conditionTable.GetCondition(defaultTransition.key);
			if (condition.IsSatisfied(stateInfo))
			{
				nextState = defaultTransition.nextState;
			}
		}

		if (transitionTable.loopTransitionDictionary.TryGetValue(CurrentState, out var loopTransition))
		{
			var condition = conditionTable.GetCondition(loopTransition.conditionType);
			if (condition.IsSatisfied(stateInfo))
			{
				nextState = CurrentState;
			}
		}

		foreach (var transition in transitionTable.transitionList)
		{
			if (transition.prevState == CurrentState)
			{
				var condition = conditionTable.GetCondition(transition.conditionType);
				if (condition.IsSatisfied(stateInfo))
				{
					nextState = transition.nextState;
				}
			}
		}

		// 키 프레임 등을 체크해서 슈퍼아머나 무적도 걸러야 합니다.

		return nextState;
	}
}
