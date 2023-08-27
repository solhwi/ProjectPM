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

	// ��ǲ�� ���� ��Ȳ�� �ִϸ����Ϳ��� �����Ѵ�.
	// �ִϸ����ʹ� ���� �������� ������Ʈ ��Ȳ�� �м��ؼ�, �ൿ�Ѵ�.

	// �������� �ൿ�� ������Ʈ �ӽ��� ���� CharacterComponent�� �����Ͽ� �����Ѵ�.

	public void Play(FrameSyncInputData inputData)
	{
		prevFrameInputData = currentFrameInputData;
		currentFrameInputData = inputData;

		var param = new CharacterParam(inputData, physicsComponent.IsGrounded, physicsComponent.Velocity);
		animatorComponent.TryChangeState(param, currentState, out currentState);
	}

	public void OnMove()
	{
		physicsComponent.Move(currentFrameInputData.MoveInput);
	}
}
