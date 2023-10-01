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
		
		public override void OnSLStateEnter(EntityMeditatorComponent owner, EntityAnimatorStateInfo animatorStateInfo)
		{
            if (owner == null)
                return;

            owner.Move(jumpVector * Time.deltaTime);
        }

		public override void OnSLStateNoTransitionUpdate(EntityMeditatorComponent owner, EntityAnimatorStateInfo animatorStateInfo)
		{
            owner.Move(jumpVector * Time.deltaTime);
        }

		public override void OnSLStateExit(EntityMeditatorComponent owner, EntityAnimatorStateInfo animatorStateInfo)
		{
            owner.Move(jumpVector * Time.deltaTime);
        }
	}

}
