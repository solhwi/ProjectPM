using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
	public class CharacterJumpUpState : CharacterAnimatorState
    {
		private float jumpYPower = 0.0f;
        private Vector2 jumpVector = new Vector2 (0, 0);
		
		public override void OnSLStateEnter(EntityMeditatorComponent owner, AnimationStateInfo<FrameSyncStateParam> stateInfo)
		{
            if (owner == null)
                return;

            var stat = characterStatTable.GetStat(owner.EntityType);
            if (stat == null)
                return;

            jumpYPower = stat.jumpPower;
            jumpVector = new Vector2(stateInfo.stateParam.userInput.moveInput.x, stateInfo.stateParam.userInput.moveInput.y * jumpYPower);

            owner.OnPostMove(jumpVector * Time.deltaTime);
        }

		public override void OnSLStateNoTransitionUpdate(EntityMeditatorComponent owner, AnimationStateInfo<FrameSyncStateParam> stateInfo)
		{
            owner.OnPostMove(jumpVector * Time.deltaTime);
        }

		public override void OnSLStateExit(EntityMeditatorComponent owner, AnimationStateInfo<FrameSyncStateParam> stateInfo)
		{
            owner.OnPostMove(jumpVector * Time.deltaTime);
        }
	}

}
