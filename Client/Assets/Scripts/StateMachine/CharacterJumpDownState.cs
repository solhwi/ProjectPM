using StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class CharacterJumpDownState : CharacterAnimatorState
    {
        private Vector2 jumpVector = new Vector2(0, 0);

        public override void OnSLStateEnter(CharacterComponent owner, FrameCommandMessage command)
        {
            if (owner == null)
                return;

            var inputMessage = command.ToInput();
            jumpVector = new Vector2(inputMessage.moveInput.x, 0);

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
