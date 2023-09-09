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

public enum CharacterState
{
	Idle, // ����
	Move, // ��, ��
	Jump, // ���� (�ϸ鼭 �������� �̵��� ����)
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

public interface IStateInput
{

}

public struct FrameSyncCharacterStateInput : IStateInput
{
	public FrameSyncInputData frameData;

	public bool IsGrounded;
	public Vector2 Velocity;

    public IEnumerable<AttackableComponent> attackers;
    public ObjectComponent defender;
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

	private FrameSyncCharacterStateInput inputData = new();

	public override void Initialize(ENUM_TEAM_TYPE teamType, bool isBoss)
	{
		base.Initialize(teamType, isBoss);

		physicsComponent = GetComponent<PhysicsComponent>();

		animatorComponent = GetComponent<CharacterAnimatorComponent>();
		animatorComponent.Initialize(this);
	}

	public void OnPlayerInput(FrameSyncInputData frameData)
	{
		inputData = new();
        inputData.frameData = frameData;
    }

	public override void OnOtherInput(IEnumerable<AttackableComponent> attackers)
	{
		if (attackers.Any() == false)
			return;

        inputData.attackers = attackers;
		inputData.defender = this;
    }

	public override void OnPostInput()
	{
        inputData.Velocity = physicsComponent.Velocity;
		inputData.IsGrounded = physicsComponent.IsGrounded;

		animatorComponent.TryChangeState(inputData);
	}

	public void OnPostMove(Vector2 moveVec)
	{
		physicsComponent.Move(moveVec);
	}

	public void OnChangeDamageBox(Vector2 damageBox)
	{
		physicsComponent.SetCollisionBox(damageBox);
	}

	public override void OnUpdateAnimation()
	{

	}
}
