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
	public FrameSyncInputData frameData = null;

	public bool IsGrounded = false;
	public Vector2 Velocity = default;

    public IEnumerable<AttackableComponent> attackers = null;
    public ObjectComponent defender = null;
}

public class FrameSyncCharacterOutputData : IStateParam
{
	public readonly Vector2 moveVec = default;

	public FrameSyncCharacterOutputData()
	{
		moveVec = default;
    }

	public FrameSyncCharacterOutputData(Vector2 moveVec)
	{
		this.moveVec = moveVec;
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

	private FrameSyncCharacterInputData inputData = new();
    private FrameSyncCharacterOutputData outputData = new();

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

	public void OnPostStateUpdate(FrameSyncCharacterOutputData output)
	{
		outputData = output;
    }

	// ���� ó���� �õ��Ѵ�.
	public override void OnUpdateAnimation()
	{
		if (outputData == null)
			return;

		physicsComponent.Move(outputData.moveVec);
		outputData = null;

    }
}
