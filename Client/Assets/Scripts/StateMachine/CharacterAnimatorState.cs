using Mono.CecilX;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
	public class CharacterAnimatorStateMachine : AnimatorState<CharacterComponent, FrameSyncStateParam, CharacterState>
	{
        private static AnimatorState<CharacterComponent, FrameSyncStateParam, CharacterState>[] animatorStates = null;

		public static void Initialize(Animator animator, CharacterComponent characterComponent)
		{
			animatorStates = animator.GetBehaviours<AnimatorState<CharacterComponent, FrameSyncStateParam, CharacterState>>();
			foreach (var state in animatorStates)
			{
                state.InternalInitialize(animator, characterComponent);
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
	}

	public class CharacterAnimatorState : AnimatorState<CharacterComponent, FrameSyncStateParam, CharacterState>
    {
        [SerializeField] protected CharacterStatTable characterStatTable = null;
        [SerializeField] protected TransitionTable transitionTable = null;
        [SerializeField] protected ConditionTable conditionTable = null;

#if UNITY_EDITOR
		protected virtual void Reset()
        {
            characterStatTable = AssetDatabase.LoadAssetAtPath<CharacterStatTable>("Assets/Bundle/Datas/Parser/CharacterStatTable.asset");
			transitionTable = AssetDatabase.LoadAssetAtPath<TransitionTable>("Assets/Bundle/Datas/Parser/TransitionTable.asset");
			conditionTable = AssetDatabase.LoadAssetAtPath<ConditionTable>("Assets/Bundle/Datas/Parser/ConditionTable.asset");
		}
#endif

        protected override bool TryChangeState(AnimationStateInfo<FrameSyncStateParam> stateInfo, CharacterState prevState, out CharacterState currentState)
        {
			foreach (var transition in transitionTable.characterTransitionList)
			{
				if (transition.prevState == prevState)
				{
					var condition = conditionTable.GetCondition(transition.conditionType);
					if (condition == null)
						continue;

					if (condition.IsSatisfied(stateInfo) == false)
						continue;

					currentState = transition.nextState;
					return true;
				}
			}

			currentState = prevState;
			return false;
		}
    }

}
