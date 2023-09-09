using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
	public class CharacterAttackState : CharacterLinkedSMB
    {
		private bool isChangeable = false;

		protected override bool TryChangeState(FrameSyncCharacterInputData inputParam, CharacterState prevState, out CharacterState currentState)
		{
			currentState = prevState;

			if (isChangeable)
			{
				var attackKey = inputParam.frameData.pressedAttackKey;
				if (attackKey == ENUM_ATTACK_KEY.ATTACK)
				{
					// currentState = CharacterState.Attack2;
				}
				else
				{
					currentState = CharacterState.Idle;
				}
			}

			return currentState != prevState;
		}

		public override void OnSLStateEnter(CharacterComponent owner, FrameSyncCharacterInputData param, AnimatorStateInfo stateInfo, AnimatorControllerPlayable controller)
		{
			isChangeable = stateInfo.normalizedTime >= 1.0f;
		}

		public override void OnSLStateNoTransitionUpdate(CharacterComponent owner, FrameSyncCharacterInputData param, AnimatorStateInfo stateInfo, AnimatorControllerPlayable controller)
		{
			isChangeable = stateInfo.normalizedTime >= 1.0f;
		}

		public override void OnSLStateExit(CharacterComponent owner, FrameSyncCharacterInputData param, AnimatorStateInfo stateInfo, AnimatorControllerPlayable controller)
		{

		}
	}

}
