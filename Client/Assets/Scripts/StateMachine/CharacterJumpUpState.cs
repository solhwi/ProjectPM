using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
	public class CharacterJumpUpState : EntityAnimatorState
    {
        private Vector2 jumpVector = new Vector2 (0, 0);
		
		public override void OnSLStateEnter(EntityMeditatorComponent owner, FrameInputSnapShotMessage message)
		{
            if (owner == null)
                return;

			var inputMessage = message.ConvertToInput();
			jumpVector = new Vector2(inputMessage.moveInput.x, inputMessage.moveInput.y * 20.0f);

			owner.Move(jumpVector * Time.deltaTime);
        }

		public override void OnSLStateNoTransitionUpdate(EntityMeditatorComponent owner, FrameInputSnapShotMessage message)
		{
            owner.Move(jumpVector * Time.deltaTime);
        }

		public override void OnSLStateExit(EntityMeditatorComponent owner, FrameInputSnapShotMessage message)
		{
            owner.Move(jumpVector * Time.deltaTime);
        }
	}

}
