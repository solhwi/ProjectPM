using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
	public class CharacterMoveState : SceneLinkedSMB<CharacterAnimatorComponent>
	{
		public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
			m_MonoBehaviour.OnStateUpdate(CharacterState.Move, frameDeltaCount);
		}
	}

}
