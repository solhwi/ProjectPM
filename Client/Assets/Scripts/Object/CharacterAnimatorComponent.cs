using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimatorComponent : AnimatorComponent<CharacterState>
{
	public void TryChangeState(CharacterParam param, CharacterState prevState, out CharacterState currentState)
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

        if (param.InputData.MoveInput.y > 0.0f || param.IsGrounded == false)
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

        if (prevState != currentState)
        {
            SetBool(prevState, false);
        }

        SetBool(currentState, true);
    }
}
