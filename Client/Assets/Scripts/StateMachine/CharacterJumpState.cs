using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
	public class CharacterJumpState : SceneLinkedSMB<CharacterAnimatorComponent>
	{
		public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
			m_MonoBehaviour.OnStateEnter(CharacterState.Jump, frameDeltaCount);
		}

		public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
			m_MonoBehaviour.OnStateUpdate(CharacterState.Jump, frameDeltaCount);
		}
		public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
			m_MonoBehaviour.OnStateExit(CharacterState.Jump, frameDeltaCount);
		}
	}

}
