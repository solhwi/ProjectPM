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

	// ��ǲ�� �ǰݺ��� ���� ó���ȴ�.
	public void OnInput(FrameSyncInputData inputData)
	{
		prevFrameInputData = currentFrameInputData;
		currentFrameInputData = inputData;

		prevState = currentState;

		var param = new CharacterInputStateParam(inputData, physicsComponent.IsGrounded, physicsComponent.MoveVector);
		animatorComponent.TryChangeState(param, prevState, out currentState);

		Debug.Log($"���� ������ : {currentFrameInputData.frameCount}, ��ǲ���� ���� ������Ʈ ���� : {currentState}");
	}

	public override void OnDamageInput(IEnumerable<AttackableComponent> attackers)
	{
		if (attackers.Any() == false)
			return;

		var param = new CharacterHitStateParam(attackers, this);
		animatorComponent.TryChangeHitState(param, prevState, out currentState);

		Debug.Log($"���� ������ : {currentFrameInputData.frameCount}, �ǰ����� ���� ���� ������Ʈ ���� : {currentState}");
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
