using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
	public class CharacterMoveState : CharacterAnimatorState
    {
		public override void OnSLStateEnter(CharacterComponent owner, FrameCommandMessage command)
		{
			var inputMessage = command.ToInput();
            Move(owner, inputMessage.moveInput.x);
		}

		public override void OnSLStateNoTransitionUpdate(CharacterComponent owner, FrameCommandMessage command)
        {
			var inputMessage = command.ToInput();
			Move(owner, inputMessage.moveInput.x);
		}

		private void Move(CharacterComponent owner, float x)
		{
			var moveVec = new Vector2(x * Time.deltaTime, 0);
			owner.AddMovement(moveVec);
		}
	}

}
