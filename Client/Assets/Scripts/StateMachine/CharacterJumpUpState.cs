using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
	public class CharacterJumpUpState : EntityAnimatorState
    {
		private float jumpYPower = 0.0f;
        private Vector2 jumpVector = new Vector2 (0, 0);
		
		public override void OnSLStateEnter(EntityMeditatorComponent owner, AnimationStateInfo animatorStateInfo, IStateInfo stateInfo)
		{
            if (owner == null)
                return;

            var stat = characterStatTable.GetStat(owner.EntityType);
            if (stat == null)
                return;

            jumpYPower = stat.jumpPower;

            var message = stateInfo.Convert<FrameInputSnapShotMessage>();
            // jumpVector = new Vector2(message.moveInput.x, message.moveInput.y * jumpYPower);

            owner.Move(jumpVector * Time.deltaTime);
        }

		public override void OnSLStateNoTransitionUpdate(EntityMeditatorComponent owner, AnimationStateInfo animatorStateInfo, IStateInfo stateInfo)
		{
            owner.Move(jumpVector * Time.deltaTime);
        }

		public override void OnSLStateExit(EntityMeditatorComponent owner, AnimationStateInfo animatorStateInfo, IStateInfo stateInfo)
		{
            owner.Move(jumpVector * Time.deltaTime);
        }
	}

}
