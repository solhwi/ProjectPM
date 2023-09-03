using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
	public class CharacterJumpState : CharacterLinkedSMB
    {
		[SerializeField] float jumpPower = 5.0f;

        private float jumpYPower = 0.0f;
		
		public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
            if (currStateParam == null)
                return;

            jumpYPower = currStateParam.frameData.MoveInput.y * jumpPower;

            var outputParam = new FrameSyncCharacterOutputData(new Vector2(currStateParam.frameData.MoveInput.x, jumpYPower) * Time.deltaTime);
            owner.OnPostStateUpdate(outputParam);
        }

		public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
            if (currStateParam == null)
                return;

            var outputParam = new FrameSyncCharacterOutputData(new Vector2(currStateParam.frameData.MoveInput.x, jumpYPower) * Time.deltaTime);
            owner.OnPostStateUpdate(outputParam);
        }

		public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
            if (currStateParam == null)
                return;

            var outputParam = new FrameSyncCharacterOutputData(new Vector2(currStateParam.frameData.MoveInput.x, jumpYPower) * Time.deltaTime);
            owner.OnPostStateUpdate(outputParam);
        }
	}

}
