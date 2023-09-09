using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

public interface IStateInfo
{

}

public struct AnimationStateInfo<TParam> : IStateInfo
{
	public readonly TParam stateParam;
	public readonly int currentAnimationKeyFrame;
	public readonly int frameDeltaCount;

	public readonly float normalizedTime;
	public readonly bool isFinished;

	public AnimationStateInfo(TParam stateParam, int currentAnimationKeyFrame, int frameDeltaCount, float normalizedTime)
	{
		this.stateParam = stateParam;
		this.currentAnimationKeyFrame = currentAnimationKeyFrame;
		this.frameDeltaCount = frameDeltaCount;
		this.normalizedTime = normalizedTime;
		this.isFinished = normalizedTime >= 1.0f;
	}
}

namespace StateMachine
{
	public class AnimatorState<TMonoBehaviour, TParam, TState> : SealedStateMachineBehaviour
        where TMonoBehaviour : MonoBehaviour
        where TState : Enum
    {
		
		private TMonoBehaviour owner;
		private TParam stateParam;

		private TState prevState;
		private TState currentState;

		private bool m_FirstFrameHappened;
		private bool m_LastFrameHappened;

		private int frameDeltaCount = 0;

		public void InternalInitialize(Animator animator, TMonoBehaviour owner)
		{
			this.owner = owner;

			var states = animator.GetBehaviours<AnimatorState<TMonoBehaviour, TParam, TState>>();
			foreach (var state in states)
			{
				state.OnInitialize(owner);
			}
		}

		public void TryInternalChangeState(TParam stateParam)
		{
			this.stateParam = stateParam;
		}

		private AnimationStateInfo<TParam> MakeAnimationStateInfo(Animator animator, AnimatorStateInfo stateInfo)
        {
            int currentKeyFrame = GetCurrentKeyFrame(animator, stateInfo);
            return new AnimationStateInfo<TParam>(stateParam, currentKeyFrame, frameDeltaCount, stateInfo.normalizedTime);
        }

        private int GetCurrentKeyFrame(Animator animator, AnimatorStateInfo stateInfo)
        {
            var currentClip = animator.GetCurrentAnimatorClipInfo(0).FirstOrDefault().clip;
            if (currentClip == null)
                return 0;

            return Mathf.FloorToInt(currentClip.length * stateInfo.normalizedTime * currentClip.frameRate);
        }

        public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            m_FirstFrameHappened = false;
            frameDeltaCount = 0;

            var animationStateInfo = MakeAnimationStateInfo(animator, stateInfo);
            OnSLStateEnter(owner, animationStateInfo);
        }

        public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            if (!animator.gameObject.activeSelf)
                return;

            frameDeltaCount++;

            var animationStateInfo = MakeAnimationStateInfo(animator, stateInfo);

            if (animator.IsInTransition(layerIndex) && animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash)
            {
                OnSLTransitionToStateUpdate(owner, animationStateInfo);
            }

            if (!animator.IsInTransition(layerIndex) && m_FirstFrameHappened)
            {
                OnSLStateNoTransitionUpdate(owner, animationStateInfo);
            }

            if (animator.IsInTransition(layerIndex) && !m_LastFrameHappened && m_FirstFrameHappened)
            {
                m_LastFrameHappened = true;

                OnSLStatePreExit(owner, animationStateInfo);
            }

            if (!animator.IsInTransition(layerIndex) && !m_FirstFrameHappened)
            {
                m_FirstFrameHappened = true;

                OnSLStatePostEnter(owner, animationStateInfo);
            }

            if (animator.IsInTransition(layerIndex) && animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash)
            {
                OnSLTransitionFromStateUpdate(owner, animationStateInfo);
            }

            if (TryChangeState(stateParam, prevState, out currentState))
            {
                Debug.LogWarning($"스테이트 변경 : {prevState} -> {currentState}");
                animator.Play(currentState.ToString());
                prevState = currentState;
            }
        }

        protected virtual bool TryChangeState(TParam input, TState prevState, out TState currentState)
        {
            currentState = prevState;
            return false;
        }

        public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            m_LastFrameHappened = false;
            frameDeltaCount++;

            var animationStateInfo = MakeAnimationStateInfo(animator, stateInfo);
            OnSLStateExit(owner, animationStateInfo);

            frameDeltaCount = 0;
		}

        /// <summary>
        /// Called by a MonoBehaviour in the scene during its Start function.
        /// </summary>
        public virtual void OnInitialize(TMonoBehaviour animator) { }

        /// <summary>
        /// Called before Updates when execution of the state first starts (on transition to the state).
        /// </summary>
        public virtual void OnSLStateEnter(TMonoBehaviour animator, AnimationStateInfo<TParam> stateInfo) { }

        /// <summary>
        /// Called after OnSLStateEnter every frame during transition to the state.
        /// </summary>
        public virtual void OnSLTransitionToStateUpdate(TMonoBehaviour animator, AnimationStateInfo<TParam> stateInfo) { }

        /// <summary>
        /// Called on the first frame after the transition to the state has finished.
        /// </summary>
        public virtual void OnSLStatePostEnter(TMonoBehaviour animator, AnimationStateInfo<TParam> stateInfo) { }

        /// <summary>
        /// Called every frame after PostEnter when the state is not being transitioned to or from.
        /// </summary>
        public virtual void OnSLStateNoTransitionUpdate(TMonoBehaviour animator, AnimationStateInfo<TParam> stateInfo) { }

        /// <summary>
        /// Called on the first frame after the transition from the state has started.  Note that if the transition has a duration of less than a frame, this will not be called.
        /// </summary>
        public virtual void OnSLStatePreExit(TMonoBehaviour animator, AnimationStateInfo<TParam> stateInfo) { }

        /// <summary>
        /// Called after OnSLStatePreExit every frame during transition to the state.
        /// </summary>
        public virtual void OnSLTransitionFromStateUpdate(TMonoBehaviour animator, AnimationStateInfo<TParam> stateInfo) { }

        /// <summary>
        /// Called after Updates when execution of the state first finshes (after transition from the state).
        /// </summary>
        public virtual void OnSLStateExit(TMonoBehaviour animator, AnimationStateInfo<TParam> stateInfo) { }

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