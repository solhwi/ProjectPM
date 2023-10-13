using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
	public class CharacterJumpUpState : CharacterAnimatorState
    {
		[SerializeField] private float jumpPower = 20.0f;
        private Vector2 jumpVector = new Vector2 (0, 0);
		
		public override void OnSLStateEnter(CharacterComponent owner, FrameCommandMessage command)
		{
            if (owner == null)
                return;

			var inputMessage = command.ToInput();
			jumpVector = new Vector2(inputMessage.moveInput.x, inputMessage.moveInput.y * jumpPower);

			owner.AddMovement(jumpVector * Time.deltaTime);
        }

		public override void OnSLStateNoTransitionUpdate(CharacterComponent owner, FrameCommandMessage command)
		{
			var inputMessage = command.ToInput();
			jumpVector.x = inputMessage.moveInput.x;

			owner.AddMovement(jumpVector * Time.deltaTime);
        }

		public override void OnSLStateExit(CharacterComponent owner, FrameCommandMessage command)
		{
			var inputMessage = command.ToInput();
			jumpVector.x = inputMessage.moveInput.x;

			owner.AddMovement(jumpVector * Time.deltaTime);
        }
	}

}
