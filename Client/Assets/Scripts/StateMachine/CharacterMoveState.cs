using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
	public class CharacterMoveState : CharacterLinkedSMB
    {
		public override void OnSLStateEnter(CharacterComponent owner, FrameSyncCharacterStateInput input, SceneLinkedAnimatorStateInfo stateInfo)
		{
			Move(owner, input.frameData.moveInput.x);
		}

		public override void OnSLStateNoTransitionUpdate(CharacterComponent owner, FrameSyncCharacterStateInput input, SceneLinkedAnimatorStateInfo stateInfo)
        {
			Move(owner, input.frameData.moveInput.x);
		}

		private void Move(CharacterComponent owner, float x)
		{
			var moveVec = new Vector2(x * Time.deltaTime, 0);
			owner.OnPostMove(moveVec);
		}
	}

}
