using Mono.CecilX;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
	public class CharacterAnimatorStateMachine : AnimatorState<CharacterComponent, FrameSyncStateParam, CharacterState>
	{
        private static AnimatorState<CharacterComponent, FrameSyncStateParam, CharacterState>[] animatorStates = null;

		public static void Initialize(Animator animator, CharacterComponent characterComponent)
		{
			animatorStates = animator.GetBehaviours<AnimatorState<CharacterComponent, FrameSyncStateParam, CharacterState>>();
			foreach (var state in animatorStates)
			{
                state.InternalInitialize(animator, characterComponent);
			}
		}

		public static void TryChangeState(FrameSyncStateParam stateParam)
		{
            if (animatorStates == null)
                return;

			foreach (var state in animatorStates)
			{
				state.TryInternalChangeState(stateParam);
			}
		}
	}

	public class CharacterAnimatorState : AnimatorState<CharacterComponent, FrameSyncStateParam, CharacterState>
    {
        [SerializeField] protected CharacterStatTable characterStatTable = null;

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            characterStatTable = AssetDatabase.LoadAssetAtPath<CharacterStatTable>("Assets/Bundle/Datas/Parser/CharacterStatTable.asset");
        }
#endif

        protected override bool TryChangeState(FrameSyncStateParam input, CharacterState prevState, out CharacterState currentState)
        {
            currentState = CharacterState.Idle;

            if (input.IsGrounded)
            {
                if (input.userInput.moveInput.x != 0.0f)
                {
                    currentState = input.userInput.isDash ? CharacterState.Dash : CharacterState.Move;
                }

                if (input.userInput.pressedAttackKey == ENUM_ATTACK_KEY.ATTACK)
                {
                    currentState = CharacterState.Attack;
                }
                else if (input.userInput.pressedAttackKey == ENUM_ATTACK_KEY.SKILL)
                {
                    currentState = CharacterState.Skill;
                }
                else if (input.userInput.pressedAttackKey == ENUM_ATTACK_KEY.ULTIMATE)
                {
                    currentState = CharacterState.Ultimate;
                }
                else if (input.userInput.isGuard)
                {
                    currentState = CharacterState.Guard;
                }
            }

            if (input.userInput.moveInput.y > 0.0f && input.IsGrounded)
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
