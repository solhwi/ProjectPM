using StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class CharacterJumpDownState : EntityAnimatorState
    {
		public override void OnSLStateEnter(EntityMeditatorComponent owner, FrameInputSnapShotMessage message)
		{
			if (owner == null)
				return;

			var inputMessage = message.ConvertToInput();
			var jumpVector = new Vector2(inputMessage.moveInput.x, 0.0f);

			owner.Move(jumpVector * Time.deltaTime);
		}

		public override void OnSLStateNoTransitionUpdate(EntityMeditatorComponent owner, FrameInputSnapShotMessage message)
		{
			var inputMessage = message.ConvertToInput();
			var jumpVector = new Vector2(inputMessage.moveInput.x, 0.0f);

			owner.Move(jumpVector * Time.deltaTime);
		}

		public override void OnSLStateExit(EntityMeditatorComponent owner, FrameInputSnapShotMessage message)
		{
			var inputMessage = message.ConvertToInput();
			var jumpVector = new Vector2(inputMessage.moveInput.x, 0.0f);

			owner.Move(jumpVector * Time.deltaTime);
		}
	}

}
