using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
	public class CharacterMoveState : CharacterAnimatorState
    {
		public override void OnSLStateEnter(CharacterComponent owner, AnimationStateInfo<FrameSyncStateParam> stateInfo)
		{
			Move(owner, stateInfo.stateParam.userInput.moveInput.x);
		}

		public override void OnSLStateNoTransitionUpdate(CharacterComponent owner,AnimationStateInfo<FrameSyncStateParam> stateInfo)
        {
			Move(owner, stateInfo.stateParam.userInput.moveInput.x);
		}

		private void Move(CharacterComponent owner, float x)
		{
			var moveVec = new Vector2(x * Time.deltaTime, 0);
			owner.OnPostMove(moveVec);
		}
	}

}
