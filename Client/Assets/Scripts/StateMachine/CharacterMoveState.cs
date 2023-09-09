using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
	public class CharacterMoveState : CharacterLinkedSMB
    {
		public override void OnSLStateEnter(CharacterComponent owner, FrameSyncCharacterInputData param, AnimatorStateInfo stateInfo, AnimatorControllerPlayable controller)
		{
			if (param == null)
				return;

			Move(owner, param.frameData.moveInput.x);
		}

		public override void OnSLStateNoTransitionUpdate(CharacterComponent owner, FrameSyncCharacterInputData param, AnimatorStateInfo stateInfo, AnimatorControllerPlayable controller)
		{
			if (param == null)
				return;

			Move(owner, param.frameData.moveInput.x);
		}

		private void Move(CharacterComponent owner, float x)
		{
			var moveVec = new Vector2(x * Time.deltaTime, 0);
			owner.OnPostMove(moveVec);
		}
	}

}
