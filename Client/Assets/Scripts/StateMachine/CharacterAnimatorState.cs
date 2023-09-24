using Mono.CecilX;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
	public class CharacterAnimatorStateMachine : AnimatorState<CharacterComponent, FrameSyncStateParam, CharacterState>
	{
        private static AnimatorState<CharacterComponent, FrameSyncStateParam, CharacterState>[] animatorStates = null;

		// Animator에 따른 구분이 필요하다.
		public static void Initialize(Animator animator, CharacterComponent characterComponent)
		{
			animatorStates = animator.GetBehaviours<AnimatorState<CharacterComponent, FrameSyncStateParam, CharacterState>>();
			foreach (var state in animatorStates)
			{
                state.InternalInitialize(characterComponent);
			}
		}

		public static void TryChangeState(FrameSyncStateParam stateParam)
		{
            if (animatorStates == null)
                return;

			foreach (var state in animatorStates)
			{
				state.TryInternalChangeState(stateParam);
			}
		}

		public static void ChangeState(CharacterState state)
		{
            if (animatorStates == null)
                return;

            foreach (var animatorState in animatorStates)
            {
                animatorState.InternalChangeState(state);
            }
        }

    }

    public class CharacterAnimatorState : AnimatorState<CharacterComponent, FrameSyncStateParam, CharacterState>
    {
        [SerializeField] protected CharacterStatTable characterStatTable = null;
        [SerializeField] protected CharacterTransitionTable transitionTable = null;
        [SerializeField] protected ConditionTable conditionTable = null;

#if UNITY_EDITOR
		protected virtual void Reset()
        {
            characterStatTable = AssetDatabase.LoadAssetAtPath<CharacterStatTable>("Assets/Bundle/Datas/Parser/CharacterStatTable.asset");
			transitionTable = AssetDatabase.LoadAssetAtPath<CharacterTransitionTable>("Assets/Bundle/Datas/Parser/CharacterTransitionTable.asset");
			conditionTable = AssetDatabase.LoadAssetAtPath<ConditionTable>("Assets/Bundle/Datas/Parser/ConditionTable.asset");
		}
#endif

        protected override bool TryChangeState(AnimationStateInfo<FrameSyncStateParam> stateInfo, CharacterState prevState, out CharacterState currentState)
        {
			currentState = prevState;

			foreach (var transition in transitionTable.transitionList)
			{
				if (transition.prevState == prevState)
				{
					var condition = conditionTable.GetCondition(transition.conditionType);
					if (condition.IsSatisfied(stateInfo))
					{
						currentState = transition.nextState;
						return true;
					}
				}
			}

			if (transitionTable.loopTransitionDictionary.TryGetValue(prevState, out var loopTransition))
			{
				var condition = conditionTable.GetCondition(loopTransition.conditionType);
				if (condition.IsSatisfied(stateInfo))
				{
					currentState = prevState;
					return false;
				}
			}

			if (transitionTable.defaultTransitionList.Any())
			{
				var defaultTransition = transitionTable.defaultTransitionList.FirstOrDefault();
				var condition = conditionTable.GetCondition(defaultTransition.key);
				if (condition.IsSatisfied(stateInfo))
				{
					currentState = defaultTransition.nextState;
				}			
			}

			bool isChanged = currentState != prevState;
            if (isChanged)
			{
                CharacterAnimatorStateMachine.ChangeState(currentState);
            }

			return isChanged;
		}
    }

}
