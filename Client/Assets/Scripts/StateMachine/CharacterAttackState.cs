using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
	public class CharacterAttackState : CharacterLinkedSMB
    {
		private bool isChangeable = false;

		protected override bool TryChangeState(FrameSyncCharacterStateInput input, CharacterState prevState, out CharacterState currentState)
		{
			currentState = prevState;

			if (isChangeable)
			{
				var attackKey = input.frameData.pressedAttackKey;
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

		public override void OnSLStateEnter(CharacterComponent owner, FrameSyncCharacterStateInput input, SceneLinkedAnimatorStateInfo stateInfo)
		{
			isChangeable = stateInfo.isFinished;
		}

		public override void OnSLStateNoTransitionUpdate(CharacterComponent owner, FrameSyncCharacterStateInput input, SceneLinkedAnimatorStateInfo stateInfo)
		{
			isChangeable = stateInfo.isFinished;
        }

		public override void OnSLStateExit(CharacterComponent owner, FrameSyncCharacterStateInput input, SceneLinkedAnimatorStateInfo stateInfo)
		{

		}
	}

}
