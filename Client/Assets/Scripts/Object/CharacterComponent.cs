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
	Idle, // ����
	Move, // ��, ��
	Jump, // ���� (�ϸ鼭 �������� �̵��� ����)
	Dash, // �뽬
	Down, // �ٿ�
	Landing, // ����
	Recovery, // ���
	Hit, // ������ ����

	Attack, // ����
	DashAttack, // �뽬 ����
	JumpAttack, // ���� ����

	Skill, // ��ų ����
	DashSkill, // �뽬 ��ų
	JumpSkill, // ���� ��ų

	Guard, // ����
	Ultimate, // �ñر�
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
			Debug.Log($"���� ������ : {currentFrameInputData.frameCount}, {prevState} -> {currentState}");
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
