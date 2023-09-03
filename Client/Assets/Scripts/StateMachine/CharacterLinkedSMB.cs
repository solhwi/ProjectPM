using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class CharacterLinkedSMB : SceneLinkedSMB<CharacterAnimatorComponent, FrameSyncCharacterInputData, CharacterState>
    {
        protected override bool TryChangeState(FrameSyncCharacterInputData inputParam, CharacterState prevState, out CharacterState currentState)
        {
            currentState = CharacterState.Idle;

            if (inputParam == null)
                return prevState != currentState;

            if (inputParam.frameData == null)
                return prevState != currentState;

            if (inputParam.IsGrounded)
            {
                if (inputParam.frameData.MoveInput.x != 0.0f)
                {
                    currentState = inputParam.frameData.isDash ? CharacterState.Dash : CharacterState.Move;
                }

                if (inputParam.frameData.PressedAttackKey == ENUM_ATTACK_KEY.ATTACK)
                {
                    currentState = CharacterState.Attack;
                }
                else if (inputParam.frameData.PressedAttackKey == ENUM_ATTACK_KEY.SKILL)
                {
                    currentState = CharacterState.Skill;
                }
                else if (inputParam.frameData.PressedAttackKey == ENUM_ATTACK_KEY.ULTIMATE)
                {
                    currentState = CharacterState.Ultimate;
                }
                else if (inputParam.frameData.isGuard)
                {
                    currentState = CharacterState.Guard;
                }
            }

            if (inputParam.frameData.MoveInput.y > 0.0f && inputParam.IsGrounded)
            {
                currentState = CharacterState.Jump;
            }
            else if (inputParam.IsGrounded == false)
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
                if (inputParam.frameData.PressedAttackKey == ENUM_ATTACK_KEY.ATTACK)
                {
                    currentState = CharacterState.DashAttack;
                }
                else if (inputParam.frameData.PressedAttackKey == ENUM_ATTACK_KEY.SKILL)
                {
                    currentState = CharacterState.DashSkill;
                }
            }
            else if (prevState == CharacterState.Jump || prevState == CharacterState.Landing) // 전 프레임에 점프 중이었다면
            {
                if (inputParam.frameData.PressedAttackKey == ENUM_ATTACK_KEY.ATTACK)
                {
                    currentState = CharacterState.JumpAttack;
                }
                else if (inputParam.frameData.PressedAttackKey == ENUM_ATTACK_KEY.SKILL)
                {
                    currentState = CharacterState.JumpSkill;
                }
            }
            else if (prevState == CharacterState.Attack) // 전 프레임에 공격 중이었다면
            {
                if (inputParam.frameData.PressedAttackKey == ENUM_ATTACK_KEY.ATTACK)
                {
                    currentState = CharacterState.Attack;
                }
            }

            return prevState != currentState;
        }
    }

}
