using StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// �� ���� Ÿ���� �߰��Ǹ� Unity Layer���� �߰��ؾ� �մϴ�.
public enum ENUM_LAYER_TYPE
{
	Map = 3,
	Ground = 6,
	Platform = 7,
	Object = 8,
	Enemy = 9,
	Boss = 10,
	Friendly = 11,
	Projectile = 12,
	UI = 13,
}

public enum ENUM_TEAM_TYPE
{
	None = -1,
	Friendly = 0,
	Enemy = 1,
}

public enum ENUM_ENTITY_TYPE
{
	None = -1,	
    RedMan = 0,
	BlueMan = 1,
	GreenMan = 2,
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
	StandHit, // ������ ����
	AirborneHit, // ���߿��� ������ ����
	Die, // ���

	Attack1, // ����
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
	public FrameSyncInputMessage userInput;

	public bool IsGrounded;
	public Vector2 Velocity;

    public IEnumerable<AttackableComponent> attackers;
    public EntityMeditatorComponent defender;

	public void Clear()
	{
		userInput = default;
		IsGrounded = false;
		Velocity = default;
		attackers = null;
		defender = null;
	}
}

[RequireComponent(typeof(RenderingComponent))]
[RequireComponent(typeof(PhysicsComponent))]
[RequireComponent(typeof(CharacterStateMachineComponent))]
[Character("EntityMeditator.prefab")]
public class EntityMeditatorComponent : EntityComponent
{
	private RenderingComponent renderingCompoonent = null;
	private PhysicsComponent physicsComponent = null;
	private CharacterStateMachineComponent stateMachineComponent = null;

	private FrameSyncStateParam stateParam = new();

	public override void Initialize(ENUM_ENTITY_TYPE type)
	{
		base.Initialize(type);

		physicsComponent = GetComponent<PhysicsComponent>();

		stateMachineComponent = GetComponent<CharacterStateMachineComponent>();
		stateMachineComponent.Initialize(this);
	}

	public void SetEntityLayer(ENUM_LAYER_TYPE layerType)
	{
		renderingCompoonent = GetComponent<RenderingComponent>();
		renderingCompoonent.Initialize(layerType, Guid);
    }

	public void OnPlayerInput(FrameSyncInputMessage frameData)
	{
		stateParam.Clear();
		stateParam.userInput = frameData;
    }

	public void OnOtherInput(IEnumerable<AttackableComponent> attackers)
	{
		if (attackers.Any() == false)
			return;

        stateParam.attackers = attackers;
		stateParam.defender = this;
    }

	public override void OnPostUpdate()
	{
        stateParam.Velocity = physicsComponent.Velocity;
		stateParam.IsGrounded = physicsComponent.IsGrounded;

		stateMachineComponent.TryChangeState(stateParam);
	}

	public void OnPostMove(Vector2 moveVec)
	{
		physicsComponent.Move(moveVec);
	}

	public override void OnLateUpdate()
	{

	}
}