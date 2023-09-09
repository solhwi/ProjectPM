using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
	public class CharacterJumpState : CharacterLinkedSMB
    {
		private float jumpYPower = 0.0f;
        private Vector2 jumpVector = new Vector2 (0, 0);
		
		public override void OnSLStateEnter(CharacterComponent owner, FrameSyncCharacterInputData param, AnimatorStateInfo stateInfo, AnimatorControllerPlayable controller)
		{
            if (owner == null)
                return;

            if (param == null)
                return;

            var stat = characterStatTable.GetStat(owner.CharacterType);
            if (stat == null)
                return;

            jumpYPower = stat.jumpPower;
			jumpVector = new Vector2(param.frameData.moveInput.x, param.frameData.moveInput.y * jumpYPower);

            owner.OnPostMove(jumpVector * Time.deltaTime);
        }

		public override void OnSLStateNoTransitionUpdate(CharacterComponent owner, FrameSyncCharacterInputData param, AnimatorStateInfo stateInfo, AnimatorControllerPlayable controller)
		{
            owner.OnPostMove(jumpVector * Time.deltaTime);
        }

		public override void OnSLStateExit(CharacterComponent owner, FrameSyncCharacterInputData param, AnimatorStateInfo stateInfo, AnimatorControllerPlayable controller)
		{
            owner.OnPostMove(jumpVector * Time.deltaTime);
        }
	}

}
