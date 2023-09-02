using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

public class CharacterInputStateParam
{
	public readonly FrameSyncInputData InputData = null;

	public readonly bool IsGrounded = false;
	public readonly Vector2 Velocity = default;

	public CharacterInputStateParam(FrameSyncInputData inputData, bool isGrounded, Vector2 velocity)
	{
		InputData = inputData;
		IsGrounded = isGrounded;
		Velocity = velocity;
	}
}

public class CharacterHitStateParam
{
	public readonly IEnumerable<AttackableComponent> attackers = null;
	public readonly ObjectComponent defender = null;

	public CharacterHitStateParam(IEnumerable<AttackableComponent> attackers, ObjectComponent defender)
	{
		this.attackers = attackers;
		this.defender = defender;
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

	private CharacterState prevState;
	private CharacterState currentState;

	private FrameSyncInputData prevFrameInputData = null;
	private FrameSyncInputData currentFrameInputData = new FrameSyncInputData();

	private Vector2 frameMoveVec = Vector2.zero;
	
	public float gravity = 50f;

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

	// 인풋이 피격보다 먼저 처리된다.
	public void OnInput(FrameSyncInputData inputData)
	{
		prevFrameInputData = currentFrameInputData;
		currentFrameInputData = inputData;

		prevState = currentState;

		var param = new CharacterInputStateParam(inputData, physicsComponent.IsGrounded, physicsComponent.MoveVector);
		animatorComponent.TryChangeState(param, prevState, out currentState);

		Debug.Log($"현재 프레임 : {currentFrameInputData.frameCount}, 인풋으로 인한 스테이트 변경 : {currentState}");
	}

	public override void OnDamageInput(IEnumerable<AttackableComponent> attackers)
	{
		if (attackers.Any() == false)
			return;

		var param = new CharacterHitStateParam(attackers, this);
		animatorComponent.TryChangeHitState(param, prevState, out currentState);

		Debug.Log($"현재 프레임 : {currentFrameInputData.frameCount}, 피격으로 인한 최종 스테이트 변경 : {currentState}");
	}

	private void OnStateEnter(CharacterState state)
	{
		switch (state)
		{
			case CharacterState.Move:
				frameMoveVec = new Vector2(currentFrameInputData.MoveInput.x, 0);
				break;
			case CharacterState.Jump:
				frameMoveVec = new Vector2(currentFrameInputData.MoveInput.x, currentFrameInputData.MoveInput.y * 105.0f * Time.deltaTime);
				break;
		}
	}

	private void OnStateUpdate(CharacterState state)
	{
		switch (state)
		{
			case CharacterState.Move:

				frameMoveVec = new Vector2(currentFrameInputData.MoveInput.x, 0);
				break;

			case CharacterState.Jump:
			case CharacterState.Landing:
				frameMoveVec.y -= physicsComponent.Gravity * Time.deltaTime;
				break;
		}

		physicsComponent.Move(frameMoveVec * Time.deltaTime);
	}

	private void OnStateExit(CharacterState state)
	{
		switch (state)
		{
			case CharacterState.Move:
				break;
		}
	}
}
