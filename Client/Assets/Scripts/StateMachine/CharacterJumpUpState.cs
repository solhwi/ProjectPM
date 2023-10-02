using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
	public class CharacterJumpUpState : EntityAnimatorState
    {
        private Vector2 jumpVector = new Vector2 (0, 0);
		
		public override void OnSLStateEnter(EntityMeditatorComponent owner, IStateMessage message)
		{
            if (owner == null)
                return;

			var inputMessage = message.ConvertToInput();
			jumpVector = inputMessage.moveInput;

			owner.Move(jumpVector * Time.deltaTime);
        }

		public override void OnSLStateNoTransitionUpdate(EntityMeditatorComponent owner, IStateMessage message)
		{
            owner.Move(jumpVector * Time.deltaTime);
        }

		public override void OnSLStateExit(EntityMeditatorComponent owner, IStateMessage message)
		{
            owner.Move(jumpVector * Time.deltaTime);
        }
	}

}
