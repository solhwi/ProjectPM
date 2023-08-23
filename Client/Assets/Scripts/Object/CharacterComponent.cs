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
	IDLE, // ����
	MOVE, // ��, ��
	JUMP, // ���� (�ϸ鼭 �������� �̵��� ����)
	DASH, // �뽬
	DOWN, // �ٿ�
	LANDING, // ����
	RECOVERY, // ���
	HIT, // ������ ����

	ATTACK, // ����
	DASH_ATTACK, // �뽬 ����
	JUMP_ATTACK, // ���� ����

	SKILL, // ��ų ����
	DASH_SKILL, // �뽬 ��ų
	JUMP_SKILL, // ���� ��ų

	GUARD, // ����
	GUARD_SKILL, // �и� �� ������ �ݰ� ��ų
	ULTIMATE, // �ñر�
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

	// ��ǲ�� ���� ��Ȳ�� �ִϸ����Ϳ��� �����Ѵ�.
	// �ִϸ����ʹ� ���� �������� ������Ʈ ��Ȳ�� �м��ؼ�, �ൿ�Ѵ�.

	// �������� �ൿ�� ������Ʈ �ӽ��� ���� CharacterComponent�� �����Ͽ� �����Ѵ�.

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
