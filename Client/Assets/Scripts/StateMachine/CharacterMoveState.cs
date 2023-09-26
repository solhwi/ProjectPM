using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
	public class CharacterMoveState : CharacterAnimatorState
    {
		public override void OnSLStateEnter(EntityMeditatorComponent owner, AnimationStateInfo<FrameSyncStateParam> stateInfo)
		{
			Move(owner, stateInfo.stateParam.userInput.moveInput.x);
		}

		public override void OnSLStateNoTransitionUpdate(EntityMeditatorComponent owner,AnimationStateInfo<FrameSyncStateParam> stateInfo)
        {
			Move(owner, stateInfo.stateParam.userInput.moveInput.x);
		}

		private void Move(EntityMeditatorComponent owner, float x)
		{
			var moveVec = new Vector2(x * Time.deltaTime, 0);
			owner.OnPostMove(moveVec);
		}
	}

}
