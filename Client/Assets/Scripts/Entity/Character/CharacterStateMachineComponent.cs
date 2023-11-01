using StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterStateMachineComponent : EntityComponent
{
	private Animator animator;
	private CharacterAnimatorState[] animatorStates;

	[SerializeField] private AddressableResourceSystem resourceSystem = null;
	[SerializeField] private SkillSystem skillSystem = null;

	[SerializeField] private CharacterTransitionTable transitionTable = null;
	[SerializeField] private CharacterConditionTable conditionTable = null;

	protected override void Reset()
	{
		base.Reset();

		resourceSystem = AssetLoadHelper.GetSystemAsset<AddressableResourceSystem>();
		skillSystem = AssetLoadHelper.GetSystemAsset<SkillSystem>();

		transitionTable = AssetLoadHelper.GetDataAsset<CharacterTransitionTable>();
		conditionTable = AssetLoadHelper.GetDataAsset<CharacterConditionTable>();
	}

	public ENUM_CHARACTER_STATE CurrentState
	{
		get;
		private set;
	} = ENUM_CHARACTER_STATE.Idle;

	public float CurrentStateNormalizedTime
	{
		get
		{
			return animator.GetCurrentNormalizedTime();
		}
	}

	public void Initialize(CharacterBehaviour owner)
	{
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
                return resourceSystem.LoadCached<RuntimeAnimatorController>("Assets/Bundle/Animation/RedMan/RedMan.overrideController");
		}

		return null;
	}

	public void PushCommand(FrameCommandMessage message)
	{
        foreach (var state in animatorStates)
        {
            state.PushCommand(message);
        }
    }

	public void ChangeState(ENUM_CHARACTER_STATE nextState)
    {
		if (CurrentState != nextState)
		{
			Debug.Log($"{Entity}의 스테이트 변경 : {CurrentState} => {nextState}");
			animator.Play(nextState.ToString());
			CurrentState = nextState;
		}
	}

	private bool IsSkillState(ENUM_CHARACTER_STATE characterState)
	{
		return characterState == ENUM_CHARACTER_STATE.Skill ||
			characterState == ENUM_CHARACTER_STATE.DashSkill ||
			characterState == ENUM_CHARACTER_STATE.JumpSkill ||
			characterState == ENUM_CHARACTER_STATE.Attack1 ||
			characterState == ENUM_CHARACTER_STATE.Attack2 ||
			characterState == ENUM_CHARACTER_STATE.Attack3 ||
			characterState == ENUM_CHARACTER_STATE.Ultimate;
	}

	public ENUM_CHARACTER_STATE GetSimulatedNextState(FrameCommandMessage snapShotMessage, ENUM_CHARACTER_STATE currentState = ENUM_CHARACTER_STATE.None)
	{
		currentState = currentState == ENUM_CHARACTER_STATE.None ? CurrentState : currentState;
		ENUM_CHARACTER_STATE nextState = currentState;

		foreach (var transition in transitionTable.transitionList)
		{
			if (transition.prevState == currentState)
			{
				var condition = conditionTable.GetCondition(transition.conditionType);
				if (condition.IsSatisfied(snapShotMessage))
				{
					if (IsSkillState(transition.nextState) == false || skillSystem.IsUsableSkillState(Entity, transition.nextState))
					{
						nextState = transition.nextState;
					}
				}
			}
		}

		return nextState;
	}
}
