using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
    public struct SceneLinkedAnimatorStateInfo
    {
        public readonly int currentAnimationKeyFrame;
        public readonly int frameDeltaCount;

        public readonly float normalizedTime;
        public readonly bool isFinished;

        public SceneLinkedAnimatorStateInfo(int currentAnimationKeyFrame, int frameDeltaCount, float normalizedTime)
        {
            this.currentAnimationKeyFrame = currentAnimationKeyFrame;
            this.frameDeltaCount = frameDeltaCount;
            this.normalizedTime = normalizedTime;
            this.isFinished = normalizedTime >= 1.0f;
        }
    }
    
    public class SceneLinkedSMB<TMonoBehaviour, TInput, TState> : SealedSMB
        where TMonoBehaviour : MonoBehaviour
        where TInput : IStateInput
        where TState : Enum
    {
        private static TState prevState;
        private static TState currentState;

        private static TMonoBehaviour owner;
        private static TInput currentFrameInput;

        bool m_FirstFrameHappened;
        bool m_LastFrameHappened;

		private int frameDeltaCount = 0;

        public static void Initialize(Animator animator, TMonoBehaviour monoBehaviour)
        {
            owner = monoBehaviour;

            var sceneLinkedSMBs = animator.GetBehaviours<SceneLinkedSMB<TMonoBehaviour, TInput, CharacterState>>();
            foreach (var smb in sceneLinkedSMBs)
            {
                smb.OnInitialize(owner);
            }
        }

        public static void TryChangeState(TInput input)
        {
            currentFrameInput = input;
        }

        private SceneLinkedAnimatorStateInfo MakeAnimationStateInfo(Animator animator, AnimatorStateInfo stateInfo)
        {
            int currentKeyFrame = GetCurrentKeyFrame(animator, stateInfo);
            return new SceneLinkedAnimatorStateInfo(currentKeyFrame, frameDeltaCount, stateInfo.normalizedTime);
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
            OnSLStateEnter(owner, currentFrameInput, animationStateInfo);
        }

        public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            if (!animator.gameObject.activeSelf)
                return;

            frameDeltaCount++;

            var animationStateInfo = MakeAnimationStateInfo(animator, stateInfo);

            if (animator.IsInTransition(layerIndex) && animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash)
            {
                OnSLTransitionToStateUpdate(owner, currentFrameInput, animationStateInfo);
            }

            if (!animator.IsInTransition(layerIndex) && m_FirstFrameHappened)
            {
                OnSLStateNoTransitionUpdate(owner, currentFrameInput, animationStateInfo);
            }

            if (animator.IsInTransition(layerIndex) && !m_LastFrameHappened && m_FirstFrameHappened)
            {
                m_LastFrameHappened = true;

                OnSLStatePreExit(owner, currentFrameInput, animationStateInfo);
            }

            if (!animator.IsInTransition(layerIndex) && !m_FirstFrameHappened)
            {
                m_FirstFrameHappened = true;

                OnSLStatePostEnter(owner, currentFrameInput, animationStateInfo);
            }

            if (animator.IsInTransition(layerIndex) && animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash)
            {
                OnSLTransitionFromStateUpdate(owner, currentFrameInput, animationStateInfo);
            }

            if (TryChangeState(currentFrameInput, prevState, out currentState))
            {
                Debug.LogWarning($"스테이트 변경 : {prevState} -> {currentState}");
                animator.Play(currentState.ToString());
                prevState = currentState;
            }
        }

        protected virtual bool TryChangeState(TInput input, TState prevState, out TState currentState)
        {
            currentState = prevState;
            return false;
        }

        public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            m_LastFrameHappened = false;
            frameDeltaCount++;

            var animationStateInfo = MakeAnimationStateInfo(animator, stateInfo);
            OnSLStateExit(owner, currentFrameInput, animationStateInfo);

            frameDeltaCount = 0;
		}

        /// <summary>
        /// Called by a MonoBehaviour in the scene during its Start function.
        /// </summary>
        public virtual void OnInitialize(TMonoBehaviour animator) { }

        /// <summary>
        /// Called before Updates when execution of the state first starts (on transition to the state).
        /// </summary>
        public virtual void OnSLStateEnter(TMonoBehaviour animator, TInput input, SceneLinkedAnimatorStateInfo stateInfo) { }

        /// <summary>
        /// Called after OnSLStateEnter every frame during transition to the state.
        /// </summary>
        public virtual void OnSLTransitionToStateUpdate(TMonoBehaviour animator, TInput input, SceneLinkedAnimatorStateInfo stateInfo) { }

        /// <summary>
        /// Called on the first frame after the transition to the state has finished.
        /// </summary>
        public virtual void OnSLStatePostEnter(TMonoBehaviour animator, TInput input, SceneLinkedAnimatorStateInfo stateInfo) { }

        /// <summary>
        /// Called every frame after PostEnter when the state is not being transitioned to or from.
        /// </summary>
        public virtual void OnSLStateNoTransitionUpdate(TMonoBehaviour animator, TInput input, SceneLinkedAnimatorStateInfo stateInfo) { }

        /// <summary>
        /// Called on the first frame after the transition from the state has started.  Note that if the transition has a duration of less than a frame, this will not be called.
        /// </summary>
        public virtual void OnSLStatePreExit(TMonoBehaviour animator, TInput input, SceneLinkedAnimatorStateInfo stateInfo) { }

        /// <summary>
        /// Called after OnSLStatePreExit every frame during transition to the state.
        /// </summary>
        public virtual void OnSLTransitionFromStateUpdate(TMonoBehaviour animator, TInput input, SceneLinkedAnimatorStateInfo stateInfo) { }

        /// <summary>
        /// Called after Updates when execution of the state first finshes (after transition from the state).
        /// </summary>
        public virtual void OnSLStateExit(TMonoBehaviour animator, TInput input, SceneLinkedAnimatorStateInfo stateInfo) { }

    }

    //This class repalce normal StateMachineBehaviour. It add the possibility of having direct reference to the object
    //the state is running on, avoiding the cost of retrienving it through a GetComponent every time.
    //c.f. Documentation for more in depth explainations.
    public abstract class SealedSMB : StateMachineBehaviour
    {
        public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
    }
}