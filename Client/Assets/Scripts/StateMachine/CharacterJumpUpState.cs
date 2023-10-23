using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
	public class CharacterJumpUpState : CharacterAnimatorState
    {
        private Vector2 jumpVector = new Vector2 (0, 0);
		private float jumpAbortSpeedReduction = 19.6f;

        public override void OnSLStateEnter(CharacterComponent owner, FrameCommandMessage command)
		{
            if (owner == null)
                return;

            var inputMessage = command.ToInput();
			var inputY = PhysicsSystem.Gravity.GetMovementYByGravity(inputMessage.moveInput.y);

            jumpVector = new Vector2(inputMessage.moveInput.x, inputY * owner.JumpPower);
			owner.AddMovement(jumpVector * Time.deltaTime);
        }

		public override void OnSLStateNoTransitionUpdate(CharacterComponent owner, FrameCommandMessage command)
		{
            var inputMessage = command.ToInput();
			jumpVector.x = inputMessage.moveInput.x;
			jumpVector.y = owner.JumpPower - frameDeltaTime * jumpAbortSpeedReduction;

			owner.AddMovement(jumpVector * Time.deltaTime);
        }

		public override void OnSLStateExit(CharacterComponent owner, FrameCommandMessage command)
		{
            var inputMessage = command.ToInput();
			jumpVector.x = inputMessage.moveInput.x;
			jumpVector.y = owner.JumpPower - frameDeltaTime * jumpAbortSpeedReduction;

            owner.AddMovement(jumpVector * Time.deltaTime);
        }
	}

}
