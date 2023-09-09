using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

namespace StateMachine
{
    public class SceneLinkedSMB<TMonoBehaviour, TParam, TState> : SealedSMB
        where TMonoBehaviour : MonoBehaviour
        where TParam : IStateParam
        where TState : Enum
    {
        private static TState prevState;
        private static TState currentState;

        private static TMonoBehaviour owner;
        private static TParam currentStateParam;

        bool m_FirstFrameHappened;
        bool m_LastFrameHappened;

		protected int frameDeltaCount = 0;

        public static void Initialize(Animator animator, TMonoBehaviour monoBehaviour)
        {
            owner = monoBehaviour;

            var sceneLinkedSMBs = animator.GetBehaviours<SceneLinkedSMB<TMonoBehaviour, TParam, CharacterState>>();
            foreach (var smb in sceneLinkedSMBs)
            {
                smb.OnInitialize(owner);
            }
        }

        public static void TryChangeState(TParam param)
        {
            currentStateParam = param;
        }

        public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            m_FirstFrameHappened = false;
            frameDeltaCount = 0;

            OnSLStateEnter(owner, currentStateParam, stateInfo);
            OnSLStateEnter(owner, currentStateParam, stateInfo, controller);
        }

        public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            if (!animator.gameObject.activeSelf)
                return;

            frameDeltaCount++;

			if (animator.IsInTransition(layerIndex) && animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash)
            {
                OnSLTransitionToStateUpdate(owner, currentStateParam, stateInfo);
                OnSLTransitionToStateUpdate(owner, currentStateParam, stateInfo, controller);
            }

            if (!animator.IsInTransition(layerIndex) && m_FirstFrameHappened)
            {
                OnSLStateNoTransitionUpdate(owner, currentStateParam, stateInfo);
                OnSLStateNoTransitionUpdate(owner, currentStateParam, stateInfo, controller);
            }

            if (animator.IsInTransition(layerIndex) && !m_LastFrameHappened && m_FirstFrameHappened)
            {
                m_LastFrameHappened = true;

                OnSLStatePreExit(owner, currentStateParam, stateInfo);
                OnSLStatePreExit(owner, currentStateParam, stateInfo, controller);
            }

            if (!animator.IsInTransition(layerIndex) && !m_FirstFrameHappened)
            {
                m_FirstFrameHappened = true;

                OnSLStatePostEnter(owner, currentStateParam, stateInfo);
                OnSLStatePostEnter(owner, currentStateParam, stateInfo, controller);
            }

            if (animator.IsInTransition(layerIndex) && animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash)
            {
                OnSLTransitionFromStateUpdate(owner, currentStateParam, stateInfo);
                OnSLTransitionFromStateUpdate(owner, currentStateParam, stateInfo, controller);
            }

            if (TryChangeState(currentStateParam, prevState, out currentState))
            {
                Debug.LogWarning($"스테이트 변경 : {prevState} -> {currentState}");
                animator.Play(currentState.ToString());
                prevState = currentState;
            }
        }

        protected virtual bool TryChangeState(TParam currentStateParam, TState prevState, out TState currentState)
        {
            currentState = prevState;
            return false;
        }

        public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            m_LastFrameHappened = false;
            frameDeltaCount++;

			OnSLStateExit(owner, currentStateParam, stateInfo);
            OnSLStateExit(owner, currentStateParam, stateInfo, controller);

            frameDeltaCount = 0;
		}

        /// <summary>
        /// Called by a MonoBehaviour in the scene during its Start function.
        /// </summary>
        public virtual void OnInitialize(TMonoBehaviour animator) { }

        /// <summary>
        /// Called before Updates when execution of the state first starts (on transition to the state).
        /// </summary>
        public virtual void OnSLStateEnter(TMonoBehaviour animator, TParam param, AnimatorStateInfo stateInfo) { }

        /// <summary>
        /// Called after OnSLStateEnter every frame during transition to the state.
        /// </summary>
        public virtual void OnSLTransitionToStateUpdate(TMonoBehaviour animator, TParam param, AnimatorStateInfo stateInfo) { }

        /// <summary>
        /// Called on the first frame after the transition to the state has finished.
        /// </summary>
        public virtual void OnSLStatePostEnter(TMonoBehaviour animator, TParam param, AnimatorStateInfo stateInfo) { }

        /// <summary>
        /// Called every frame after PostEnter when the state is not being transitioned to or from.
        /// </summary>
        public virtual void OnSLStateNoTransitionUpdate(TMonoBehaviour animator, TParam param, AnimatorStateInfo stateInfo) { }

        /// <summary>
        /// Called on the first frame after the transition from the state has started.  Note that if the transition has a duration of less than a frame, this will not be called.
        /// </summary>
        public virtual void OnSLStatePreExit(TMonoBehaviour animator, TParam param, AnimatorStateInfo stateInfo) { }

        /// <summary>
        /// Called after OnSLStatePreExit every frame during transition to the state.
        /// </summary>
        public virtual void OnSLTransitionFromStateUpdate(TMonoBehaviour animator, TParam param, AnimatorStateInfo stateInfo) { }

        /// <summary>
        /// Called after Updates when execution of the state first finshes (after transition from the state).
        /// </summary>
        public virtual void OnSLStateExit(TMonoBehaviour animator, TParam param, AnimatorStateInfo stateInfo) { }

        /// <summary>
        /// Called before Updates when execution of the state first starts (on transition to the state).
        /// </summary>
        public virtual void OnSLStateEnter(TMonoBehaviour animator, TParam param, AnimatorStateInfo stateInfo, AnimatorControllerPlayable controller) { }

        /// <summary>
        /// Called after OnSLStateEnter every frame during transition to the state.
        /// </summary>
        public virtual void OnSLTransitionToStateUpdate(TMonoBehaviour animator, TParam param, AnimatorStateInfo stateInfo, AnimatorControllerPlayable controller) { }

        /// <summary>
        /// Called on the first frame after the transition to the state has finished.
        /// </summary>
        public virtual void OnSLStatePostEnter(TMonoBehaviour animator, TParam param, AnimatorStateInfo stateInfo, AnimatorControllerPlayable controller) { }

        /// <summary>
        /// Called every frame when the state is not being transitioned to or from.
        /// </summary>
        public virtual void OnSLStateNoTransitionUpdate(TMonoBehaviour animator, TParam param, AnimatorStateInfo stateInfo, AnimatorControllerPlayable controller) { }

        /// <summary>
        /// Called on the first frame after the transition from the state has started.  Note that if the transition has a duration of less than a frame, this will not be called.
        /// </summary>
        public virtual void OnSLStatePreExit(TMonoBehaviour animator, TParam param, AnimatorStateInfo stateInfo, AnimatorControllerPlayable controller) { }

        /// <summary>
        /// Called after OnSLStatePreExit every frame during transition to the state.
        /// </summary>
        public virtual void OnSLTransitionFromStateUpdate(TMonoBehaviour animator, TParam param, AnimatorStateInfo stateInfo, AnimatorControllerPlayable controller) { }

        /// <summary>
        /// Called after Updates when execution of the state first finshes (after transition from the state).
        /// </summary>
        public virtual void OnSLStateExit(TMonoBehaviour animator, TParam param, AnimatorStateInfo stateInfo, AnimatorControllerPlayable controller) { }
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