using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
	public class CharacterJumpState : CharacterLinkedSMB
    {
		public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
			
		}

		public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
            if (currStateParam == null)
                return;

			currStateParam.frameData.MoveInput.Normalize();

            var outputParam = new FrameSyncCharacterOutputData(currStateParam.frameData.MoveInput * Time.deltaTime);
            owner.OnPostStateUpdate(outputParam);
        }

		public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
			
		}
	}

}
