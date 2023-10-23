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
	private CharacterComponent characterComponent;

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
        characterComponent = owner;	
        CurrentState = ENUM_CHARACTER_STATE.Idle;

        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = LoadAnimatorController(owner.EntityType);

        animatorStates = animator.GetBehaviours<CharacterAnimatorState>();
		foreach (var state in animatorStates)
		{
			state.Initialize(owner);
		}
    }

	private RuntimeAnimatorController LoadAnimatorController(ENUM_ENTITY_TYPE entityType)
	{
		switch(entityType)
		{
			case ENUM_ENTITY_TYPE.RedMan:
			case ENUM_ENTITY_TYPE.PencilMan:
				return AddressabeResourceSystem.Instance.LoadCached<RuntimeAnimatorController>("Assets/Bundle/Animation/RedMan/RedMan.overrideController");
		}

		return null;
	}

	public void SendCommandToStateMachine(FrameCommandMessage message)
	{
        foreach (var state in animatorStates)
        {
            state.SendCommand(message);
        }
    }

	public bool ChangeState(ENUM_CHARACTER_STATE nextState)
    {
		bool isChangedState = CurrentState != nextState;
		if (isChangedState)
		{
			Debug.Log($"{Time.frameCount}에 스테이트 변경 : {CurrentState} => {nextState}");
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

		return currentState;
	}
}
