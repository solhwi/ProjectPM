using StateMachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 여기서 애니메이션 프레임 정보도 빼오면 되겠다!

[RequireComponent(typeof(Animator))]
public class EntityStateMachineComponent : MonoBehaviour
{
	private Animator animator;
	private EntityAnimatorState[] animatorStates;

	[SerializeField] private CharacterTransitionTable transitionTable = null;
	[SerializeField] private ConditionTable conditionTable = null;

	public ENUM_ENTITY_STATE CurrentState
	{
		get;
		private set;
	}

	public void Initialize(EntityMeditatorComponent owner)
	{
		animator = GetComponent<Animator>();
		animatorStates = animator.GetBehaviours<EntityAnimatorState>();

		foreach (var state in animatorStates)
		{
			state.Initialize(owner);
		}
	}

    public void ChangeState(ENUM_ENTITY_STATE nextState)
    {
		CurrentState = nextState;
		animator.Play(nextState.ToString());
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

		return nextState;
	}
}
