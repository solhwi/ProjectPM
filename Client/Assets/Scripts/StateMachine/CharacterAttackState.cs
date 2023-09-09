using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
	public class CharacterAttackState : CharacterAnimatorState
    {
		private bool isChangeable = false;

		protected override bool TryChangeState(FrameSyncStateParam input, CharacterState prevState, out CharacterState currentState)
		{
			currentState = prevState;

			if (isChangeable)
			{
				var attackKey = input.userInput.pressedAttackKey;
				if (attackKey == ENUM_ATTACK_KEY.ATTACK)
				{
					currentState = CharacterState.Attack2;
				}
				else
				{
					currentState = CharacterState.Idle;
				}
			}

			return currentState != prevState;
		}

		public override void OnSLStateEnter(CharacterComponent owner, AnimationStateInfo<FrameSyncStateParam> stateInfo)
		{
			isChangeable = stateInfo.isFinished;
		}

		public override void OnSLStateNoTransitionUpdate(CharacterComponent owner, AnimationStateInfo<FrameSyncStateParam> stateInfo)
		{
			isChangeable = stateInfo.isFinished;
        }
	}

}
