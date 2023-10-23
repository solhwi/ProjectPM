using StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


[Serializable]
public enum ENUM_CHARACTER_STATE
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

[RequireComponent(typeof(RenderingComponent))]
[RequireComponent(typeof(PhysicsComponent))]
[RequireComponent(typeof(CharacterStateMachineComponent))]
[EntityAttribute("Character.prefab")]
public class CharacterComponent : EntityComponent
{
	[SerializeField] private RenderingComponent renderingComponent = null;
	[SerializeField] private PhysicsComponent physicsComponent = null;
	[SerializeField] private CharacterStateMachineComponent stateMachineComponent = null;
	[SerializeField] private CharacterStatTable statTable = null;

	public float JumpPower => statTable.GetStat(EntityType)?.jumpPower ?? 0.0f;

	public override bool IsLeftDirection => renderingComponent.IsLeftDirection;

    public override Vector2 Velocity => physicsComponent.Velocity;

	public override Vector2 HitBox => physicsComponent.HitBox;

	public override bool IsGrounded => physicsComponent.CheckGrounded();

	public override Vector2 HitOffset => physicsComponent.HitOffset;

	public override int CurrentState => (int)stateMachineComponent.CurrentState;

	public override float CurrentNormalizedTime => stateMachineComponent.CurrentNormalizedTime;


    public override void Initialize(int ownerGuid, int entityGuid, ENUM_ENTITY_TYPE type, bool isPlayer)
	{
		base.Initialize(ownerGuid, entityGuid, type, isPlayer);
		stateMachineComponent.Initialize(this);
	}

	public override void SetEntityLayer(ENUM_LAYER_TYPE layerType)
	{
		base.SetEntityLayer(layerType);
		renderingComponent.Initialize(layerType, EntityGuid);
    }

    protected override int GetSimulatedNextState(ICommand command)
    {
		var frameMessage = command.ToFrameMessage();
		var entity = frameMessage.ToEntity();
        return (int)stateMachineComponent.GetSimulatedNextState(frameMessage, (ENUM_CHARACTER_STATE)entity.entityState);
    }

	public override bool SendCommand(ICommand command)
    {
		var frameMessage = command.ToFrameMessage();
        var nextState = GetSimulatedNextState(frameMessage);

        stateMachineComponent.SendCommandToStateMachine(frameMessage);
        return stateMachineComponent.ChangeState((ENUM_CHARACTER_STATE)nextState);
	}

	public void AddMovement(Vector2 moveVec)
	{
        physicsComponent.AddMovement(moveVec);
		SetDirection(physicsComponent.Velocity); // ������ �̵������� üũ�մϴ�.
	}
	
	public void SetDirection(Vector2 moveVec)
	{
        renderingComponent.Look(moveVec.x);
	}
}
