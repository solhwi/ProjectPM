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
public enum ENUM_ENTITY_STATE
{
	None = 0,
	Idle = 1, // ����
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

// ��ƼƼ�� �� ������Ʈ�� ������ �Ѵ�.
// ��ƼƼ �Ŵ��� - ��ƼƼ - ��ƼƼ ������Ʈ
// ���踦 ����, ��ƼƼ �Ŵ����� ��ǲ�� ���� ��ƼƼ�� ���·� ��ġ�� �����Ѵ�.
// ��ġ�� ������� �浹�� �����Ѵ�.
// ��ǲ, ��ƼƼ, �浹 + �ִϸ��̼� key frame ���� ���� ���¸� �����Ѵ�.

// component�� entity�� ������� view�� �ݿ��Ѵ�.

// �̰� ��������� �ѵ�...
// 1. �׷� �浹�� ���� �κ��� ���� �����ؾ� �Ѵ�.
// 2. �ִϸ��̼ǵ� ���� �����ؼ� ������ ������ ������ �Ѵ�.

// �и��� ���ϱ�� ����...
// Entity �����͸� �и��ϴ� �� �ƴ϶� EntityComponent�� ��ӹ޴� �������� ����.
// Entity�� ����� ������ ������ �����Ѵ�.

[RequireComponent(typeof(RenderingComponent))]
[RequireComponent(typeof(PhysicsComponent))]
[RequireComponent(typeof(EntityStateMachineComponent))]
[EntityAttribute("EntityMeditator.prefab")]
public class EntityMeditatorComponent : EntityComponent
{
	[SerializeField] private RenderingComponent renderingComponent = null;
	[SerializeField] private PhysicsComponent physicsComponent = null;
	[SerializeField] private EntityStateMachineComponent stateMachineComponent = null;
	[SerializeField] private DamageableComponent damageableComponent = null;

    public override Vector2 Velocity => physicsComponent.Velocity;

	public override Vector2 HitBox => physicsComponent.HitBox;

	public override bool IsGrounded => physicsComponent.IsGrounded;

	public override Vector2 Offset => physicsComponent.hitOffset;

	public override ENUM_ENTITY_STATE CurrentState => stateMachineComponent.CurrentState;

	public override int CurrentKeyFrame => stateMachineComponent.CurrentKeyFrame;

	public override float CurrentNormalizedTime => stateMachineComponent.CurrentNormalizedTime;

	public override void Initialize(int ownerGuid, ENUM_ENTITY_TYPE type)
	{
		base.Initialize(ownerGuid, type);
		stateMachineComponent.Initialize(this);
	}

	public void SetEntityLayer(ENUM_LAYER_TYPE layerType)
	{
		renderingComponent.Initialize(layerType, Guid);
    }

    public override ENUM_ENTITY_STATE GetSimulatedNextState(FrameInputSnapShotMessage snapShotMessage)
    {
        var entityFrameInfo = snapShotMessage.ConvertToEntity();
        return stateMachineComponent.GetSimulatedNextState(snapShotMessage, (ENUM_ENTITY_STATE)entityFrameInfo.entityState);
    }

	public override bool TryChangeState(FrameInputSnapShotMessage snapShotMessage)
    {
        var nextState = GetSimulatedNextState(snapShotMessage);
		stateMachineComponent.ChangeState(nextState, snapShotMessage);
		return true;
	}

    public void Move(Vector2 moveVec)
	{
		physicsComponent.Move(moveVec);
	}

	public override void Teleport(Vector2 posVec)
	{
		physicsComponent.Teleport(posVec);
	}

	public void MakeInvincible(bool isEnable)
	{
		damageableComponent.isEnable = isEnable;
	}

	public void MakeSuperArmor(bool isEnable)
	{

	}
}