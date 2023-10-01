using Mono.CecilX;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
	public class EntityAnimatorStateMachine : AnimatorState<EntityMeditatorComponent, ENUM_ENTITY_STATE>
	{
        private static Dictionary<Animator, AnimatorState<EntityMeditatorComponent, ENUM_ENTITY_STATE>[]> animatorStateDictionary = new Dictionary<Animator, AnimatorState<EntityMeditatorComponent, ENUM_ENTITY_STATE>[]>();

		// Animator에 따른 구분이 필요하다.
		public static void Initialize(Animator animator, EntityMeditatorComponent entityComponent)
		{
			var animatorStates = animator.GetBehaviours<AnimatorState<EntityMeditatorComponent, ENUM_ENTITY_STATE>>();
            animatorStateDictionary[animator] = animatorStates;

            foreach (var state in animatorStates)
			{
                state.Initialize(entityComponent);
			}
		}

		public static new void TryChangeState(Animator animator, ENUM_ENTITY_STATE nextState, IStateInfo info)
		{
            if (animatorStateDictionary.Any() == false)
                return;

			if (animatorStateDictionary.ContainsKey(animator) == false)
				return;

			foreach (var state in animatorStateDictionary[animator])
			{
				state.TryChangeState(animator, nextState, info);
			}
		}
	}

	public class EntityAnimatorState : AnimatorState<EntityMeditatorComponent, ENUM_ENTITY_STATE>
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
	}

}
