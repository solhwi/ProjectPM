using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;
using System;

public class CharacterAnimatorComponent : AnimatorComponent<CharacterState>
{
    public event Action<CharacterState> OnCharacterStateEnter;
	public event Action<CharacterState> OnCharacterStateUpdate;
	public event Action<CharacterState> OnCharacterStateExit;
    
	protected override void Awake()
	{
        base.Awake();
		SceneLinkedSMB<CharacterAnimatorComponent>.Initialize(animator, this);
	}

	public void TryChangeState(CharacterInputStateParam param, CharacterState prevState, out CharacterState currentState)
	{
        if (CanTransition() == false)
        {
            currentState = prevState;
            return;
        }

        currentState = CharacterState.Idle;

        if (param == null)
			return;

		if (param.InputData == null)
			return;

		SetDirection(param.InputData.MoveInput);

        if (param.IsGrounded)
        {
            if (param.InputData.MoveInput.x != 0.0f)
            {
                currentState = param.InputData.isDash ? CharacterState.Dash : CharacterState.Move;
            }

            if (param.InputData.PressedAttackKey == ENUM_ATTACK_KEY.ATTACK)
            {
                currentState = CharacterState.Attack;
            }
            else if (param.InputData.PressedAttackKey == ENUM_ATTACK_KEY.SKILL)
            {
                currentState = CharacterState.Skill;
            }
            else if (param.InputData.PressedAttackKey == ENUM_ATTACK_KEY.ULTIMATE)
            {
                currentState = CharacterState.Ultimate;
            }
            else if (param.InputData.isGuard)
            {
                currentState = CharacterState.Guard;
            }
        }

        if (param.InputData.MoveInput.y > 0.0f && param.IsGrounded)
        {
            currentState = CharacterState.Jump;
        }
        else if(param.IsGrounded == false)
        {
			currentState = param.Velocity.y >= 0.0f ? CharacterState.Jump : CharacterState.Landing;
		}

        if (prevState == CharacterState.Dash) // 전 프레임에 대쉬 중이었다면,
        {
            if (param.InputData.PressedAttackKey == ENUM_ATTACK_KEY.ATTACK)
            {
                currentState = CharacterState.DashAttack;
            }
            else if (param.InputData.PressedAttackKey == ENUM_ATTACK_KEY.SKILL)
            {
                currentState = CharacterState.DashSkill;
            }
        }
        else if (prevState == CharacterState.Jump || prevState == CharacterState.Landing) // 전 프레임에 점프 중이었다면
        {
            if (param.InputData.PressedAttackKey == ENUM_ATTACK_KEY.ATTACK)
            {
                currentState = CharacterState.JumpAttack;
            }
            else if (param.InputData.PressedAttackKey == ENUM_ATTACK_KEY.SKILL)
            {
                currentState = CharacterState.JumpSkill;
            }
        }
        else if (prevState == CharacterState.Attack) // 전 프레임에 공격 중이었다면
        {
            if (param.InputData.PressedAttackKey == ENUM_ATTACK_KEY.ATTACK)
            {
                currentState = CharacterState.Attack;
            }
        }

		SetBool(prevState, false);

		if (prevState != currentState)
        {
			SetBool(currentState, true);
		}
	}

    public void TryChangeHitState(CharacterHitStateParam param, CharacterState prevState, out CharacterState currentState)
    { 
        // attackers 의 특성과 defender의 특성에 따라 조금 더 바뀔 수 있음

		if (CanTransition() == false)
		{
			currentState = prevState;
			return;
		}
        
        if(prevState == CharacterState.Down || prevState == CharacterState.Recovery || prevState == CharacterState.Guard || prevState == CharacterState.Ultimate)
        {
			currentState = prevState;
			return;
		}

		currentState = CharacterState.Hit;
	}

	protected override bool CanTransition()
	{
		return true;
	}

	public void OnStateEnter(CharacterState state)
    {
		OnCharacterStateEnter?.Invoke(state);
	}

    public void OnStateUpdate(CharacterState state)
    {
		OnCharacterStateUpdate?.Invoke(state);
	}

    public void OnStateExit(CharacterState state)
    {
		OnCharacterStateExit?.Invoke(state);
	}
}
