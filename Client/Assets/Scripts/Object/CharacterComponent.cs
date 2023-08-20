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
	MOVE, // �⺻ ������
	JUMP, // ���� (�ϸ鼭 �������� �̵��� ����)
	DASH, // �뽬
	WEAK, // ��
	MIDDLE, // ��
	STRONG, // ��
	DASH_WEAK, // �뽬 �� �� ���� ��Ȳ
	COMMAND_MIDDLE, // �Ϻ� ������ MOVE �� MIDDLE (ĳ���� ���� ���� ����)
	COMMAND_STRONG, // �Ϻ� ������ MOVE �� STRONG (ĳ���� ���� ���� ����)
	ULTIMATE, // �ñر�
}

[RequireComponent(typeof(PhysicsComponent))]
[RequireComponent(typeof(CharacterAnimatorComponent))]
public class CharacterComponent : ObjectComponent
{
    [SerializeField] private CharacterStatTable characterStatTable = null;
	[SerializeField] private PhysicsComponent physicsComponent = null;
	[SerializeField] private CharacterAnimatorComponent animatorComponent = null;
	[SerializeField] private ENUM_CHARACTER_TYPE characterType;

	// ĳ���� �ִϸ����Ϳ� ���������� ���� ��Ȳ�� �˾ƿ´�.
	// ���� ������Ʈ�� ���� �ൿ�� ������ ���� �ְ�, �Ұ����� ���� �ִ�.
	// �̿� ���� ������� �����Ѵ�.
	// ������� ���� ��ǲ sender�� �̸� �޾� �������� ��, ���� ���� ������ �� �ֵ��� �Ѵ�.

	public bool TryAction(FrameSyncInputData inputData)
	{
		physicsComponent.Move(inputData.MoveInput);
		return true;
	}
}
