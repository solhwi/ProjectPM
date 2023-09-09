using Mono.CecilX;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    public class CharacterLinkedSMB : SceneLinkedSMB<CharacterComponent, FrameSyncCharacterStateInput, CharacterState>
    {
        [SerializeField] protected CharacterStatTable characterStatTable = null;

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            characterStatTable = AssetDatabase.LoadAssetAtPath<CharacterStatTable>("Assets/Bundle/Datas/Parser/CharacterStatTable.asset");
        }
#endif

        protected override bool TryChangeState(FrameSyncCharacterStateInput input, CharacterState prevState, out CharacterState currentState)
        {
            currentState = CharacterState.Idle;

            if (input.IsGrounded)
            {
                if (input.frameData.moveInput.x != 0.0f)
                {
                    currentState = input.frameData.isDash ? CharacterState.Dash : CharacterState.Move;
                }

                if (input.frameData.pressedAttackKey == ENUM_ATTACK_KEY.ATTACK)
                {
                    currentState = CharacterState.Attack;
                }
                else if (input.frameData.pressedAttackKey == ENUM_ATTACK_KEY.SKILL)
                {
                    currentState = CharacterState.Skill;
                }
                else if (input.frameData.pressedAttackKey == ENUM_ATTACK_KEY.ULTIMATE)
                {
                    currentState = CharacterState.Ultimate;
                }
                else if (input.frameData.isGuard)
                {
                    currentState = CharacterState.Guard;
                }
            }

            if (input.frameData.moveInput.y > 0.0f && input.IsGrounded)
            {
                currentState = CharacterState.Jump;
            }
            else if (input.IsGrounded == false)
            {
                if (input.Velocity.y > 0.01f)
                {
                    currentState = CharacterState.Jump;
                }
                else if (input.Velocity.y < -1 * 0.01f)
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
