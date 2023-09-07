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

public interface IStateParam
{

}


/// <summary>
/// ��ǲ > ��ǲ�� �ڽ��� �����͸� ���� > FSM�� �ѱ� > FSM�� ������ ������� �����͸� ������ > ����Ʈ ������Ʈ���� �並 ������
/// 
/// 
/// </summary>
public class FrameSyncCharacterInputData : IStateParam
{
	public FrameSyncInputData frameData;

	public bool IsGrounded = false;
	public Vector2 Velocity = default;

    public IEnumerable<AttackableComponent> attackers = null;
    public ObjectComponent defender = null;
}

[RequireComponent(typeof(PhysicsComponent))]
[RequireComponent(typeof(CharacterAnimatorComponent))]
public abstract class CharacterComponent : ObjectComponent
{
	private PhysicsComponent physicsComponent = null;
	private CharacterAnimatorComponent animatorComponent = null;

	public abstract ENUM_CHARACTER_TYPE CharacterType
	{
		get;
	}

	private FrameSyncCharacterInputData inputData = new();

	public override void Initialize(ENUM_TEAM_TYPE teamType, bool isBoss)
	{
		base.Initialize(teamType, isBoss);

		physicsComponent = GetComponent<PhysicsComponent>();

		animatorComponent = GetComponent<CharacterAnimatorComponent>();
		animatorComponent.Initialize(this);
	}

	public void OnPlayerInput(FrameSyncInputData frameData)
	{
		inputData ??= new();
        inputData.frameData = frameData;
    }

	public override void OnOtherInput(IEnumerable<AttackableComponent> attackers)
	{
		if (attackers.Any() == false)
			return;

        inputData ??= new();
        inputData.attackers = attackers;
		inputData.defender = this;
    }

	public override void OnPostInput()
	{
        inputData ??= new();
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
