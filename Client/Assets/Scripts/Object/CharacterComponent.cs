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

	private FrameSyncInputData currentFrameInputData = new FrameSyncInputData();
	
	private CharacterInputStateParam currentFrameInputParam = null;
	private CharacterHitStateParam currentFrameHitParam = null;

	private int jumpLastFrame = 0;

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

	public void OnPlayerInput(FrameSyncInputData inputData)
	{
		prevState = currentState;

		currentFrameInputData = inputData;
		currentFrameInputParam = new CharacterInputStateParam(inputData, physicsComponent.IsGrounded, physicsComponent.Velocity);
	}

	public override void OnOtherInput(IEnumerable<AttackableComponent> attackers)
	{
		if (attackers.Any() == false)
			return;

		currentFrameHitParam = new CharacterHitStateParam(attackers, this);
	}

	public override void OnPostInput()
	{
 		animatorComponent.TryChangeState(currentFrameInputParam, currentFrameHitParam, prevState, out currentState);

		if (prevState != currentState)
		{
			Debug.Log($"현재 프레임 : {currentFrameInputData.frameCount}, {prevState} -> {currentState}");
			animatorComponent.Play(currentState);
		}
	}

	private void OnStateEnter(CharacterState state, int frameDeltaCount)
	{
		var frameMoveVec = currentFrameInputData.MoveInput;

		switch (state)
		{
			case CharacterState.Move:
				frameMoveVec = new Vector2(frameMoveVec.x, 0);
				physicsComponent.Move(frameMoveVec * Time.deltaTime);

				break;
			case CharacterState.Jump:
				physicsComponent.Move(frameMoveVec * Time.deltaTime);
				break;
		}

	}

	private void OnStateUpdate(CharacterState state, int frameDeltaCount)
	{
		var frameMoveVec = currentFrameInputData.MoveInput;

		switch (state)
		{
			case CharacterState.Move:

				frameMoveVec = new Vector2(frameMoveVec.x, 0);
				physicsComponent.Move(frameMoveVec * Time.deltaTime);
				break;

			case CharacterState.Jump:
				frameMoveVec = new Vector2(frameMoveVec.x, 1 - (physicsComponent.Gravity * (frameDeltaCount)));
				physicsComponent.Move(frameMoveVec * Time.deltaTime);
				break;

			case CharacterState.Landing:
				frameMoveVec = new Vector2(frameMoveVec.x, 1 - (physicsComponent.Gravity * (frameDeltaCount + jumpLastFrame)));
				physicsComponent.Move(frameMoveVec * Time.deltaTime);
				break;
		}
	}

	private void OnStateExit(CharacterState state, int frameDeltaCount)
	{
		switch (state)
		{
			case CharacterState.Jump:
				jumpLastFrame = frameDeltaCount;
				break;
		}
	}
}
