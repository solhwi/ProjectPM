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
		
		public override void OnSLStateEnter(CharacterComponent owner, FrameSyncCharacterStateInput input, SceneLinkedAnimatorStateInfo stateInfo)
		{
            if (owner == null)
                return;

            var stat = characterStatTable.GetStat(owner.CharacterType);
            if (stat == null)
                return;

            jumpYPower = stat.jumpPower;
			jumpVector = new Vector2(input.frameData.moveInput.x, input.frameData.moveInput.y * jumpYPower);

            owner.OnPostMove(jumpVector * Time.deltaTime);
        }

		public override void OnSLStateNoTransitionUpdate(CharacterComponent owner, FrameSyncCharacterStateInput input, SceneLinkedAnimatorStateInfo stateInfo)
		{
            owner.OnPostMove(jumpVector * Time.deltaTime);
        }

		public override void OnSLStateExit(CharacterComponent owner, FrameSyncCharacterStateInput input, SceneLinkedAnimatorStateInfo stateInfo)
		{
            owner.OnPostMove(jumpVector * Time.deltaTime);
        }
	}

}
