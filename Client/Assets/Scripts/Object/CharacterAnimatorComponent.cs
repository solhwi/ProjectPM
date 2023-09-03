using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;
using System;

public class CharacterAnimatorComponent : AnimatorComponent<CharacterState>
{
    public event Action<CharacterState, int> OnCharacterStateEnter;
	public event Action<CharacterState, int> OnCharacterStateUpdate;
	public event Action<CharacterState, int> OnCharacterStateExit;
    
	protected override void Awake()
	{
        base.Awake();
		SceneLinkedSMB<CharacterAnimatorComponent>.Initialize(animator, this);
	}

	public void TryChangeState(CharacterInputStateParam inputParam, CharacterHitStateParam hitParam, CharacterState prevState, out CharacterState currentState)
	{
        if (CanTransition() == false)
        {
            currentState = prevState;
            return;
        }

        currentState = CharacterState.Idle;

        if (inputParam == null)
			return;

		if (inputParam.InputData == null)
			return;

        if (inputParam.IsGrounded)
        {
            if (inputParam.InputData.MoveInput.x != 0.0f)
            {
                currentState = inputParam.InputData.isDash ? CharacterState.Dash : CharacterState.Move;
            }

            if (inputParam.InputData.PressedAttackKey == ENUM_ATTACK_KEY.ATTACK)
            {
                currentState = CharacterState.Attack;
            }
            else if (inputParam.InputData.PressedAttackKey == ENUM_ATTACK_KEY.SKILL)
            {
                currentState = CharacterState.Skill;
            }
            else if (inputParam.InputData.PressedAttackKey == ENUM_ATTACK_KEY.ULTIMATE)
            {
                currentState = CharacterState.Ultimate;
            }
            else if (inputParam.InputData.isGuard)
            {
                currentState = CharacterState.Guard;
            }
        }

        if (inputParam.InputData.MoveInput.y > 0.0f && inputParam.IsGrounded)
        {
            currentState = CharacterState.Jump;
        }
        else if(inputParam.IsGrounded == false)
        {
            if (inputParam.Velocity.y > 0.01f)
            {
                currentState = CharacterState.Jump;
            }
            else if (inputParam.Velocity.y < -1 * 0.01f)
            {
				currentState = CharacterState.Landing;
			}
            else
            {
                currentState = prevState == CharacterState.Jump || prevState == CharacterState.Landing ? prevState : CharacterState.Landing;
            }
		}

        if (prevState == CharacterState.Dash) // 전 프레임에 대쉬 중이었다면,
        {
            if (inputParam.InputData.PressedAttackKey == ENUM_ATTACK_KEY.ATTACK)
            {
                currentState = CharacterState.DashAttack;
            }
            else if (inputParam.InputData.PressedAttackKey == ENUM_ATTACK_KEY.SKILL)
            {
                currentState = CharacterState.DashSkill;
            }
        }
        else if (prevState == CharacterState.Jump || prevState == CharacterState.Landing) // 전 프레임에 점프 중이었다면
        {
            if (inputParam.InputData.PressedAttackKey == ENUM_ATTACK_KEY.ATTACK)
            {
                currentState = CharacterState.JumpAttack;
            }
            else if (inputParam.InputData.PressedAttackKey == ENUM_ATTACK_KEY.SKILL)
            {
                currentState = CharacterState.JumpSkill;
            }
        }
        else if (prevState == CharacterState.Attack) // 전 프레임에 공격 중이었다면
        {
            if (inputParam.InputData.PressedAttackKey == ENUM_ATTACK_KEY.ATTACK)
            {
                currentState = CharacterState.Attack;
            }
        }
	}

	protected override bool CanTransition()
	{
        // animator.GetCurrentAnimatorStateInfo(0).IsName()
		return true;
	}

	public void OnStateEnter(CharacterState state, int frameCount)
    {
		OnCharacterStateEnter?.Invoke(state, frameCount);
	}

    public void OnStateUpdate(CharacterState state, int frameCount)
    {
		OnCharacterStateUpdate?.Invoke(state, frameCount);
	}

    public void OnStateExit(CharacterState state, int frameCount)
    {
		OnCharacterStateExit?.Invoke(state, frameCount);
	}
}
