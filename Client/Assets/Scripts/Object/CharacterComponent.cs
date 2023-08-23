using System;
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
	MOVE, // 좌, 우
	JUMP, // 점프 (하면서 물리적인 이동이 가능)
	DASH, // 대쉬
	DOWN, // 다운
	LANDING, // 착지
	RECOVERY, // 기상
	HIT, // 데미지 입음

	ATTACK, // 공격
	DASH_ATTACK, // 대쉬 공격
	JUMP_ATTACK, // 점프 공격

	SKILL, // 스킬 공격
	DASH_SKILL, // 대쉬 스킬
	JUMP_SKILL, // 점프 스킬

	GUARD, // 가드
	GUARD_SKILL, // 패링 시 나가는 반격 스킬
	ULTIMATE, // 궁극기
}

public class CharacterParam
{
	public readonly FrameSyncInputData InputData = null;

	public readonly bool IsGrounded = false;
	public readonly bool IsCeilinged = false;
	public readonly Vector2 Velocity = default;

	public CharacterParam(FrameSyncInputData inputData, bool isGrounded, bool isCeilinged, Vector2 velocity)
	{
		InputData = inputData;
		IsGrounded = isGrounded;
		IsCeilinged = isCeilinged;
		Velocity = velocity;
	}
}



[RequireComponent(typeof(PhysicsComponent))]
[RequireComponent(typeof(CharacterAnimatorComponent))]
public class CharacterComponent : ObjectComponent
{
    [SerializeField] private CharacterStatTable characterStatTable = null;
	[SerializeField] private PhysicsComponent physicsComponent = null;
	[SerializeField] private CharacterAnimatorComponent animatorComponent = null;
	[SerializeField] private ENUM_CHARACTER_TYPE characterType;

	private CharacterState currentState;

	private FrameSyncInputData prevFrameInputData = null;
	private FrameSyncInputData currentFrameInputData = new FrameSyncInputData();

	// 인풋과 물리 상황을 애니메이터에게 전달한다.
	// 애니메이터는 현재 프레임의 스테이트 상황을 분석해서, 행동한다.

	// 직접적인 행동은 스테이트 머신이 각자 CharacterComponent를 참조하여 수행한다.

	public void Play(FrameSyncInputData inputData)
	{
		prevFrameInputData = currentFrameInputData;
		currentFrameInputData = inputData;

		var param = new CharacterParam(inputData, physicsComponent.IsGrounded, physicsComponent.IsCeilinged, physicsComponent.Velocity);
		animatorComponent.TryChangeState(param, out currentState);
	}

	public void OnMove()
	{
		physicsComponent.Move(currentFrameInputData.MoveInput);
	}
}
