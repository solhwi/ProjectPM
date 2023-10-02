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

[RequireComponent(typeof(RenderingComponent))]
[RequireComponent(typeof(PhysicsComponent))]
[RequireComponent(typeof(EntityStateMachineComponent))]
[EntityAttribute("EntityMeditator.prefab")]
public class EntityMeditatorComponent : EntityComponent
{
	[SerializeField] private RenderingComponent renderingComponent = null;
	[SerializeField] private PhysicsComponent physicsComponent = null;
	[SerializeField] private EntityStateMachineComponent stateMachineComponent = null;

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

    public override ENUM_ENTITY_STATE GetSimulatedNextState(IStateMessage stateInfo)
    {
		return stateMachineComponent.GetSimulatedNextState(stateInfo);
    }

	public override bool TryChangeState(IStateMessage stateMessage)
    {
		var entityFrameInfo = stateMessage.ConvertToEntity();
		if (entityFrameInfo.entityState > 0)
		{
			stateMachineComponent.ChangeState((ENUM_ENTITY_STATE)entityFrameInfo.entityState);
		}

		var nextState = GetSimulatedNextState(stateMessage);

		bool isChanged = CurrentState != nextState;
		if (isChanged)
		{
            stateMachineComponent.ChangeState(nextState, stateMessage);
        }

        return isChanged;
	}

    public void Move(Vector2 moveVec)
	{
		physicsComponent.Move(moveVec);
	}

	public override void Teleport(Vector2 posVec)
	{
		physicsComponent.Teleport(posVec);
	}
}
