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
	Idle, // 멈춤
	Move, // 좌, 우
	Jump, // 점프 (하면서 물리적인 이동이 가능)
	Dash, // 대쉬
	Down, // 다운
	Landing, // 착지
	Recovery, // 기상
	Hit, // 데미지 입음

	Attack, // 공격
	DashAttack, // 대쉬 공격
	JumpAttack, // 점프 공격

	Skill, // 스킬 공격
	DashSkill, // 대쉬 스킬
	JumpSkill, // 점프 스킬

	Guard, // 가드
	Ultimate, // 궁극기
}

public class CharacterParam
{
	public readonly FrameSyncInputData InputData = null;

	public readonly bool IsGrounded = false;
	public readonly bool IsCeilinged = false;
	public readonly Vector2 Velocity = default;

	public CharacterParam(FrameSyncInputData inputData, bool isGrounded, Vector2 velocity)
	{
		InputData = inputData;
		IsGrounded = isGrounded;
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

	public override void Initialize()
	{
		animatorComponent.OnCharacterStateEnter += OnStateEnter;
		animatorComponent.OnCharacterStateUpdate += OnStateUpdate;
		animatorComponent.OnCharacterStateExit += OnStateExit;
	}

	public override void Clear()
	{
		animatorComponent.OnCharacterStateEnter -= OnStateEnter;
		animatorComponent.OnCharacterStateUpdate -= OnStateUpdate;
		animatorComponent.OnCharacterStateExit -= OnStateExit;
	}

	public void OnInput(FrameSyncInputData inputData)
	{
		prevFrameInputData = currentFrameInputData;
		currentFrameInputData = inputData;

		var param = new CharacterParam(inputData, physicsComponent.IsGrounded, physicsComponent.Velocity);
		animatorComponent.TryChangeState(param, currentState, out currentState);

		Debug.Log($"현재 프레임 : {currentFrameInputData.frameCount}, 스테이트 : {currentState}");
	}

	public override void OnDamageInput(IEnumerable<AttackableComponent> attackers)
	{
		// 본인의 방어력과 함께 사용 
	}

	private void OnStateEnter(CharacterState state)
	{
		switch (state)
		{
			case CharacterState.Move:
				break;
		}
	}

	private void OnStateUpdate(CharacterState state)
	{
		switch(state)
		{
			case CharacterState.Move:
				OnMove();
				break;
		}
	}

	private void OnStateExit(CharacterState state)
	{
		switch (state)
		{
			case CharacterState.Move:
				break;
		}
	}


	public void OnMove()
	{
		physicsComponent.Move(currentFrameInputData.MoveInput * Time.deltaTime);
	}
}
