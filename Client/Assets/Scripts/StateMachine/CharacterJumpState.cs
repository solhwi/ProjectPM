using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
	public class CharacterJumpState : CharacterLinkedSMB
    {
		[SerializeField] private CharacterStatTable characterStatTable = null;

		private float jumpYPower = 0.0f;
        private Vector2 jumpVector = new Vector2 (0, 0);
		
		public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
            if (owner == null)
                return;

            if (currStateParam == null)
                return;

            var stat = characterStatTable.GetStat(owner.CharacterType);
            if (stat == null)
                return;

            jumpYPower = stat.jumpPower;
			jumpVector = new Vector2(currStateParam.frameData.moveInput.x, currStateParam.frameData.moveInput.y * jumpYPower);

            owner.OnPostMove(jumpVector * Time.deltaTime);
        }

		public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
            owner.OnPostMove(jumpVector * Time.deltaTime);
        }

		public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
            owner.OnPostMove(jumpVector * Time.deltaTime);
        }
	}

}
