using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
	public class CharacterMoveState : CharacterLinkedSMB
    {
		public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
			if (currStateParam == null)
				return;

			Move(currStateParam.frameData.moveInput.x);
		}

		public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
			if (currStateParam == null)
				return;

			Move(currStateParam.frameData.moveInput.x);
		}

		private void Move(float x)
		{
			var moveVec = new Vector2(x * Time.deltaTime, 0);
			owner.OnPostMove(moveVec);
		}
	}

}
