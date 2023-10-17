using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations;

public static class AnimatorExtension
{
    public static float GetCurrentNormalizedTime(this Animator animator)
    {
		var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.normalizedTime;
	}
}

namespace StateMachine
{
    public class CharacterAnimatorState : EntityAnimatorState<CharacterComponent, FrameCommandMessage>
    {

    }

	public class EntityAnimatorState<TOwner, TCommand> : SealedStateMachineBehaviour
        where TOwner : EntityComponent
        where TCommand : ICommand
	{	
		private TOwner owner;
        private TCommand command;

		private bool m_FirstFrameHappened;
		private bool m_LastFrameHappened;

		protected int frameDeltaCount = 0;
        protected float frameDeltaTime = 0;

		public void Initialize(TOwner owner)
        {
			this.owner = owner;
            OnInitialize(owner);
        }

        public void AddCommand(TCommand command)
        {
            this.command = command;
		}

        private void FlushCommand()
        {
            command = default;
        }

		public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            m_FirstFrameHappened = false;
            frameDeltaCount = 0;
            frameDeltaTime = 0;

            OnSLStateEnter(owner, command);
            FlushCommand();
		}

        public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            if (!animator.gameObject.activeSelf)
                return;

            frameDeltaCount++;
            frameDeltaTime += Time.deltaTime;

            if (animator.IsInTransition(layerIndex) && animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == animatorStateInfo.fullPathHash)
            {
                OnSLTransitionToStateUpdate(owner, command);
            }

            if (!animator.IsInTransition(layerIndex) && m_FirstFrameHappened)
            {
                OnSLStateNoTransitionUpdate(owner, command);
            }

            if (animator.IsInTransition(layerIndex) && !m_LastFrameHappened && m_FirstFrameHappened)
            {
                m_LastFrameHappened = true;

                OnSLStatePreExit(owner, command);
            }

            if (!animator.IsInTransition(layerIndex) && !m_FirstFrameHappened)
            {
                m_FirstFrameHappened = true;

                OnSLStatePostEnter(owner, command);
            }

            if (animator.IsInTransition(layerIndex) && animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == animatorStateInfo.fullPathHash)
            {
                OnSLTransitionFromStateUpdate(owner, command);
            }

			FlushCommand();
		}

		public sealed override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            m_LastFrameHappened = false;
            frameDeltaCount++;

			OnSLStateExit(owner, command);
			FlushCommand();

			frameDeltaCount = 0;
            frameDeltaTime = 0;

        }

        /// <summary>
        /// Called by a MonoBehaviour in the scene during its Start function.
        /// </summary>
        public virtual void OnInitialize(TOwner animator) { }

        /// <summary>
        /// Called before Updates when execution of the state first starts (on transition to the state).
        /// </summary>
        public virtual void OnSLStateEnter(TOwner owner, TCommand command) { }

        /// <summary>
        /// Called after OnSLStateEnter every frame during transition to the state.
        /// </summary>
        public virtual void OnSLTransitionToStateUpdate(TOwner owner, TCommand command) { }

        /// <summary>
        /// Called on the first frame after the transition to the state has finished.
        /// </summary>
        public virtual void OnSLStatePostEnter(TOwner owner, TCommand command) { }

        /// <summary>
        /// Called every frame after PostEnter when the state is not being transitioned to or from.
        /// </summary>
        public virtual void OnSLStateNoTransitionUpdate(TOwner owner, TCommand command) { }

        /// <summary>
        /// Called on the first frame after the transition from the state has started.  Note that if the transition has a duration of less than a frame, this will not be called.
        /// </summary>
        public virtual void OnSLStatePreExit(TOwner owner, TCommand command) { }

        /// <summary>
        /// Called after OnSLStatePreExit every frame during transition to the state.
        /// </summary>
        public virtual void OnSLTransitionFromStateUpdate(TOwner owner, TCommand command) { }

        /// <summary>
        /// Called after Updates when execution of the state first finshes (after transition from the state).
        /// </summary>
        public virtual void OnSLStateExit(TOwner owner, TCommand command) { }

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