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
        private static SceneLinkedSMB<TMonoBehaviour, TParam, CharacterState>[] sceneLinkedSMBs = null;

		private static TState prevState;
		private static TState currState;

		protected TMonoBehaviour owner;
        protected TParam currStateParam;

        bool m_FirstFrameHappened;
        bool m_LastFrameHappened;

		protected int frameDeltaCount = 0;

		public static void Initialize(Animator animator, TMonoBehaviour monoBehaviour)
        {
            sceneLinkedSMBs = animator.GetBehaviours<SceneLinkedSMB<TMonoBehaviour, TParam, CharacterState>>();

            for (int i = 0; i < sceneLinkedSMBs.Length; i++)
            {
                sceneLinkedSMBs[i].InternalInitialize(animator, monoBehaviour);
            }
        }

        public static void TryChangeState(TParam param)
        {
            for (int i = 0; i < sceneLinkedSMBs.Length; i++)
            {
                sceneLinkedSMBs[i].currStateParam = param;
            }
        }

        protected void InternalInitialize(Animator animator, TMonoBehaviour monoBehaviour)
        {
            owner = monoBehaviour;
            OnStart(animator);
        }

        public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            m_FirstFrameHappened = false;
            frameDeltaCount = 0;

            OnSLStateEnter(animator, stateInfo, layerIndex);
            OnSLStateEnter(animator, stateInfo, layerIndex, controller);
        }

        public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            if (!animator.gameObject.activeSelf)
                return;

            frameDeltaCount++;

			if (animator.IsInTransition(layerIndex) && animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash)
            {
                OnSLTransitionToStateUpdate(animator, stateInfo, layerIndex);
                OnSLTransitionToStateUpdate(animator, stateInfo, layerIndex, controller);
            }

            if (!animator.IsInTransition(layerIndex) && m_FirstFrameHappened)
            {
                OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
                OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex, controller);
            }

            if (animator.IsInTransition(layerIndex) && !m_LastFrameHappened && m_FirstFrameHappened)
            {
                m_LastFrameHappened = true;

                OnSLStatePreExit(animator, stateInfo, layerIndex);
                OnSLStatePreExit(animator, stateInfo, layerIndex, controller);
            }

            if (!animator.IsInTransition(layerIndex) && !m_FirstFrameHappened)
            {
                m_FirstFrameHappened = true;

                OnSLStatePostEnter(animator, stateInfo, layerIndex);
                OnSLStatePostEnter(animator, stateInfo, layerIndex, controller);
            }

            if (animator.IsInTransition(layerIndex) && animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash)
            {
                OnSLTransitionFromStateUpdate(animator, stateInfo, layerIndex);
                OnSLTransitionFromStateUpdate(animator, stateInfo, layerIndex, controller);
            }

            if (TryChangeState(currStateParam, prevState, out currState))
            {
                Debug.LogWarning($"������Ʈ ���� : {prevState} -> {currState}");
                animator.Play(currState.ToString());
                prevState = currState;
            }
        }

        protected virtual bool TryChangeState(TParam inputParam, TState prevState, out TState currentState)
        {
            currentState = prevState;

            return currState.Equals(prevState) == false;
        }

        public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            m_LastFrameHappened = false;
            frameDeltaCount++;

			OnSLStateExit(animator, stateInfo, layerIndex);
            OnSLStateExit(animator, stateInfo, layerIndex, controller);

            frameDeltaCount = 0;
		}

        /// <summary>
        /// Called by a MonoBehaviour in the scene during its Start function.
        /// </summary>
        public virtual void OnStart(Animator animator) { }

        /// <summary>
        /// Called before Updates when execution of the state first starts (on transition to the state).
        /// </summary>
        public virtual void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        /// <summary>
        /// Called after OnSLStateEnter every frame during transition to the state.
        /// </summary>
        public virtual void OnSLTransitionToStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        /// <summary>
        /// Called on the first frame after the transition to the state has finished.
        /// </summary>
        public virtual void OnSLStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        /// <summary>
        /// Called every frame after PostEnter when the state is not being transitioned to or from.
        /// </summary>
        public virtual void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        /// <summary>
        /// Called on the first frame after the transition from the state has started.  Note that if the transition has a duration of less than a frame, this will not be called.
        /// </summary>
        public virtual void OnSLStatePreExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        /// <summary>
        /// Called after OnSLStatePreExit every frame during transition to the state.
        /// </summary>
        public virtual void OnSLTransitionFromStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        /// <summary>
        /// Called after Updates when execution of the state first finshes (after transition from the state).
        /// </summary>
        public virtual void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        /// <summary>
        /// Called before Updates when execution of the state first starts (on transition to the state).
        /// </summary>
        public virtual void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller) { }

        /// <summary>
        /// Called after OnSLStateEnter every frame during transition to the state.
        /// </summary>
        public virtual void OnSLTransitionToStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller) { }

        /// <summary>
        /// Called on the first frame after the transition to the state has finished.
        /// </summary>
        public virtual void OnSLStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller) { }

        /// <summary>
        /// Called every frame when the state is not being transitioned to or from.
        /// </summary>
        public virtual void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller) { }

        /// <summary>
        /// Called on the first frame after the transition from the state has started.  Note that if the transition has a duration of less than a frame, this will not be called.
        /// </summary>
        public virtual void OnSLStatePreExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller) { }

        /// <summary>
        /// Called after OnSLStatePreExit every frame during transition to the state.
        /// </summary>
        public virtual void OnSLTransitionFromStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller) { }

        /// <summary>
        /// Called after Updates when execution of the state first finshes (after transition from the state).
        /// </summary>
        public virtual void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller) { }
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