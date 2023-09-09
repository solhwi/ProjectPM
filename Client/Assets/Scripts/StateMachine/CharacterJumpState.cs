using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
	public class CharacterJumpState : CharacterAnimatorState
    {
		private float jumpYPower = 0.0f;
        private Vector2 jumpVector = new Vector2 (0, 0);
		
		public override void OnSLStateEnter(CharacterComponent owner, AnimationStateInfo<FrameSyncStateParam> stateInfo)
		{
            if (owner == null)
                return;

            var stat = characterStatTable.GetStat(owner.CharacterType);
            if (stat == null)
                return;

            jumpYPower = stat.jumpPower;
            jumpVector = new Vector2(stateInfo.stateParam.userInput.moveInput.x, stateInfo.stateParam.userInput.moveInput.y * jumpYPower);

            owner.OnPostMove(jumpVector * Time.deltaTime);
        }

		public override void OnSLStateNoTransitionUpdate(CharacterComponent owner, AnimationStateInfo<FrameSyncStateParam> stateInfo)
		{
            owner.OnPostMove(jumpVector * Time.deltaTime);
        }

		public override void OnSLStateExit(CharacterComponent owner, AnimationStateInfo<FrameSyncStateParam> stateInfo)
		{
            owner.OnPostMove(jumpVector * Time.deltaTime);
        }
	}

}
