using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
	public class CharacterJumpUpState : CharacterAnimatorState
    {
        private Vector2 jumpVector = new Vector2 (0, 0);
		
		public override void OnSLStateEnter(CharacterComponent owner, FrameCommandMessage command)
		{
            if (owner == null)
                return;

			var inputMessage = command.ToInput();
			jumpVector = new Vector2(inputMessage.moveInput.x, inputMessage.moveInput.y * 20.0f);

			owner.AddMovement(jumpVector * Time.deltaTime);
        }

		public override void OnSLStateNoTransitionUpdate(CharacterComponent owner, FrameCommandMessage command)
		{
            owner.AddMovement(jumpVector * Time.deltaTime);
        }

		public override void OnSLStateExit(CharacterComponent owner, FrameCommandMessage command)
		{
            owner.AddMovement(jumpVector * Time.deltaTime);
        }
	}

}
