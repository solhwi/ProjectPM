using StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static CharacterStatTable;

public enum ENUM_CHARACTER_TYPE
{
    Normal = 0,
}

[Serializable]
public enum CharacterState
{
	Idle, // ����
	Move, // ��, ��
	JumpUp, // ���� (�ϸ鼭 �������� �̵��� ����)
	JumpDown, // ���� �ٿ�
	Dash, // �뽬
	Down, // �ٿ�
	Landing, // ����
	Recovery, // ���
	Hit, // ������ ����

	Attack, // ����
	Attack2,
	Attack3,
	DashAttack, // �뽬 ����
	JumpAttack, // ���� ����

	Skill, // ��ų ����
	DashSkill, // �뽬 ��ų
	JumpSkill, // ���� ��ų

	Guard, // ����
	Ultimate, // �ñر�
}

public struct FrameSyncStateParam
{
	public FrameSyncInputData userInput;

	public bool IsGrounded;
	public Vector2 Velocity;

    public IEnumerable<AttackableComponent> attackers;
    public ObjectComponent defender;

	public void Clear()
	{
		userInput = default;
		IsGrounded = false;
		Velocity = default;
		attackers = null;
		defender = null;
	}
}

[RequireComponent(typeof(PhysicsComponent))]
[RequireComponent(typeof(CharacterAnimatorComponent))]
public abstract class CharacterComponent : ObjectComponent
{
	public abstract ENUM_CHARACTER_TYPE CharacterType
	{
		get;
	}

	private PhysicsComponent physicsComponent = null;
	private CharacterAnimatorComponent animatorComponent = null;

	private FrameSyncStateParam stateParam = new();

	public override void Initialize(ENUM_TEAM_TYPE teamType, bool isBoss)
	{
		base.Initialize(teamType, isBoss);

		physicsComponent = GetComponent<PhysicsComponent>();

		animatorComponent = GetComponent<CharacterAnimatorComponent>();
		animatorComponent.Initialize(this);
	}

	public void OnPlayerInput(FrameSyncInputData frameData)
	{
		stateParam.Clear();
		stateParam.userInput = frameData;
    }

	public override void OnOtherInput(IEnumerable<AttackableComponent> attackers)
	{
		if (attackers.Any() == false)
			return;

        stateParam.attackers = attackers;
		stateParam.defender = this;
    }

	public override void OnPostInput()
	{
        stateParam.Velocity = physicsComponent.Velocity;
		stateParam.IsGrounded = physicsComponent.IsGrounded;

		animatorComponent.TryChangeState(stateParam);
	}

	public void OnPostMove(Vector2 moveVec)
	{
		physicsComponent.Move(moveVec);
	}

	public override void OnUpdateAnimation()
	{

	}
}
