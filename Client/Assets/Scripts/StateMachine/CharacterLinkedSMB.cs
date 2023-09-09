using Mono.CecilX;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    public class CharacterLinkedSMB : SceneLinkedSMB<CharacterComponent, FrameSyncCharacterInputData, CharacterState>
    {
        [SerializeField] protected CharacterStatTable characterStatTable = null;

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            characterStatTable = AssetDatabase.LoadAssetAtPath<CharacterStatTable>("Assets/Bundle/Datas/Parser/CharacterStatTable.asset");
        }
#endif

        protected override bool TryChangeState(FrameSyncCharacterInputData inputParam, CharacterState prevState, out CharacterState currentState)
        {
            currentState = CharacterState.Idle;

            if (inputParam == null)
                return prevState != currentState;

            if (inputParam.IsGrounded)
            {
                if (inputParam.frameData.moveInput.x != 0.0f)
                {
                    currentState = inputParam.frameData.isDash ? CharacterState.Dash : CharacterState.Move;
                }

                if (inputParam.frameData.pressedAttackKey == ENUM_ATTACK_KEY.ATTACK)
                {
                    currentState = CharacterState.Attack;
                }
                else if (inputParam.frameData.pressedAttackKey == ENUM_ATTACK_KEY.SKILL)
                {
                    currentState = CharacterState.Skill;
                }
                else if (inputParam.frameData.pressedAttackKey == ENUM_ATTACK_KEY.ULTIMATE)
                {
                    currentState = CharacterState.Ultimate;
                }
                else if (inputParam.frameData.isGuard)
                {
                    currentState = CharacterState.Guard;
                }
            }

            if (inputParam.frameData.moveInput.y > 0.0f && inputParam.IsGrounded)
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
                    currentState = CharacterState.JumpDown;
                }
                else
                {
                    currentState = prevState == CharacterState.Jump || prevState == CharacterState.JumpDown ? prevState : CharacterState.JumpDown;
                }
            }

            return prevState != currentState;
        }
    }

}
