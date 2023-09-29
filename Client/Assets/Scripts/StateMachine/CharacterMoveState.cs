using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
	public class CharacterMoveState : EntityAnimatorState
    {
		public override void OnSLStateEnter(EntityMeditatorComponent owner, AnimationStateInfo animatorStateInfo, IStateInfo stateInfo)
		{
			var message = stateInfo.Convert<FrameSyncInputMessage>();
            Move(owner, message.moveInput.x);
		}

		public override void OnSLStateNoTransitionUpdate(EntityMeditatorComponent owner, AnimationStateInfo animatorStateInfo, IStateInfo stateInfo)
        {
            var message = stateInfo.Convert<FrameSyncInputMessage>();
            Move(owner, message.moveInput.x);
        }

		private void Move(EntityMeditatorComponent owner, float x)
		{
			var moveVec = new Vector2(x * Time.deltaTime, 0);
			owner.OnPostMove(moveVec);
		}
	}

}
