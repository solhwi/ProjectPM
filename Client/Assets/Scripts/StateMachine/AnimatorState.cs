using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations;

public static class AnimatorExtension
{
    private const int myLayerIndex = 0;

	public static int GetCurrentKeyFrame(this Animator animator)
	{
        var stateInfo = animator.GetCurrentAnimatorStateInfo(myLayerIndex);

		var currentClip = animator.GetCurrentAnimatorClipInfo(myLayerIndex).FirstOrDefault().clip;
		if (currentClip == null)
			return 0;

		return Mathf.FloorToInt(currentClip.length * stateInfo.normalizedTime * currentClip.frameRate);
	}

    public static float GetCurrentNormalizedTime(this Animator animator)
    {
		var stateInfo = animator.GetCurrentAnimatorStateInfo(myLayerIndex);
        return stateInfo.normalizedTime;
	}
}

namespace StateMachine
{
	public class EntityAnimatorState : SealedStateMachineBehaviour
    {	
		private EntityMeditatorComponent owner;
        private IStateMessage message;

		private bool m_FirstFrameHappened;
		private bool m_LastFrameHappened;

		private int frameDeltaCount = 0;

		public void Initialize(EntityMeditatorComponent owner)
        {
			this.owner = owner;
            OnInitialize(owner);
        }

        public void SetStateMessage(IStateMessage message)
        {
            this.message = message;
		}

        private FrameEntityAnimationMessage MakeFrameMessage(Animator animator)
        {
			var message = new FrameEntityAnimationMessage();
            message.keyFrame = animator.GetCurrentKeyFrame();
            message.normalizedTime = animator.GetCurrentNormalizedTime();

			return message;
		}

		public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            m_FirstFrameHappened = false;
            frameDeltaCount = 0;

			OnSLStateEnter(owner, message);
        }

        public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            if (!animator.gameObject.activeSelf)
                return;

            frameDeltaCount++;

			if (animator.IsInTransition(layerIndex) && animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == animatorStateInfo.fullPathHash)
            {
                OnSLTransitionToStateUpdate(owner, message);
            }

            if (!animator.IsInTransition(layerIndex) && m_FirstFrameHappened)
            {
                OnSLStateNoTransitionUpdate(owner, message);
            }

            if (animator.IsInTransition(layerIndex) && !m_LastFrameHappened && m_FirstFrameHappened)
            {
                m_LastFrameHappened = true;

                OnSLStatePreExit(owner, message);
            }

            if (!animator.IsInTransition(layerIndex) && !m_FirstFrameHappened)
            {
                m_FirstFrameHappened = true;

                OnSLStatePostEnter(owner, message);
            }

            if (animator.IsInTransition(layerIndex) && animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == animatorStateInfo.fullPathHash)
            {
                OnSLTransitionFromStateUpdate(owner, message);
            }

			// 애니메이션 처리로 인한 스테이트 변경을 시도한다.
			var animationMessage = MakeFrameMessage(animator);
			owner.TryChangeState(animationMessage);
		}

		public sealed override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            m_LastFrameHappened = false;
            frameDeltaCount++;

			OnSLStateExit(owner, message);

            frameDeltaCount = 0;
		}

        /// <summary>
        /// Called by a MonoBehaviour in the scene during its Start function.
        /// </summary>
        public virtual void OnInitialize(EntityMeditatorComponent animator) { }

        /// <summary>
        /// Called before Updates when execution of the state first starts (on transition to the state).
        /// </summary>
        public virtual void OnSLStateEnter(EntityMeditatorComponent animator, IStateMessage message) { }

        /// <summary>
        /// Called after OnSLStateEnter every frame during transition to the state.
        /// </summary>
        public virtual void OnSLTransitionToStateUpdate(EntityMeditatorComponent animator, IStateMessage message) { }

        /// <summary>
        /// Called on the first frame after the transition to the state has finished.
        /// </summary>
        public virtual void OnSLStatePostEnter(EntityMeditatorComponent animator, IStateMessage message) { }

        /// <summary>
        /// Called every frame after PostEnter when the state is not being transitioned to or from.
        /// </summary>
        public virtual void OnSLStateNoTransitionUpdate(EntityMeditatorComponent animator, IStateMessage message) { }

        /// <summary>
        /// Called on the first frame after the transition from the state has started.  Note that if the transition has a duration of less than a frame, this will not be called.
        /// </summary>
        public virtual void OnSLStatePreExit(EntityMeditatorComponent animator, IStateMessage message) { }

        /// <summary>
        /// Called after OnSLStatePreExit every frame during transition to the state.
        /// </summary>
        public virtual void OnSLTransitionFromStateUpdate(EntityMeditatorComponent animator, IStateMessage message) { }

        /// <summary>
        /// Called after Updates when execution of the state first finshes (after transition from the state).
        /// </summary>
        public virtual void OnSLStateExit(EntityMeditatorComponent animator, IStateMessage message) { }

    }

    //This class repalce normal StateMachineBehaviour. It add the possibility of having direct reference to the object
    //the state is running on, avoiding the cost of retrienving it through a GetComponent every time.
    //c.f. Documentation for more in depth explainations.
    public abstract class SealedStateMachineBehaviour : StateMachineBehaviour
    {
        public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
    }
}