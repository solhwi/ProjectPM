using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum ENUM_CHARACTER_TYPE
{
    Normal = 0,
}

public enum CharacterState
{
	IDLE, // 멈춤
	MOVE, // 기본 움직임
	JUMP, // 점프 (하면서 물리적인 이동이 가능)
	DASH, // 대쉬
	WEAK, // 약
	STRONG, // 강
	DASH_WEAK, // 대쉬 중 약 공격 상황
	COMMAND_STRONG, // 일부 방향의 MOVE 중 STRONG (캐릭터 마다 방향 상이)
	RANGED_WEAPON, // 무기를 든 상태
	ULTIMATE, // 궁극기
}

[RequireComponent(typeof(PhysicsComponent))]
[RequireComponent(typeof(CharacterAnimatorComponent))]
public class CharacterComponent : ObjectComponent
{
    [SerializeField] private CharacterStatTable characterStatTable = null;
	[SerializeField] private PhysicsComponent physicsComponent = null;
	[SerializeField] private CharacterAnimatorComponent animatorComponent = null;
	[SerializeField] private ENUM_CHARACTER_TYPE characterType;

	// 캐릭터 애니메이터와 피직스에서 현재 상황을 알아온다.
	// 현재 스테이트에 따라 행동이 가능할 수도 있고, 불가능할 수도 있다.
	// 이에 대한 결과값을 리턴한다.
	// 결과값을 받은 인풋 sender는 이를 받아 재전송할 지, 버릴 지를 결정할 수 있도록 한다.

	public bool TryAction(FrameSyncInputData inputData)
	{
		physicsComponent.Move(inputData.MoveInput);
		return true;
	}
}
