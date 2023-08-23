using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimatorComponent : AnimatorComponent<CharacterState>
{
	// 못 가는 곳을 정의함.
	Dictionary<CharacterState, CharacterState[]> characterStateMap = new Dictionary<CharacterState, CharacterState[]>()
	{
		{ CharacterState.IDLE,  new CharacterState[] { } },
		{ CharacterState.MOVE,  new CharacterState[] { CharacterState.RECOVERY,  } },
	};

	public void TryChangeState(CharacterParam param, out CharacterState currentState)
	{
		currentState = CharacterState.MOVE;
	}
}
