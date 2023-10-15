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

	public override bool IsLeftDirection => renderingComponent.IsLeftDirection;

    public override Vector2 Velocity => physicsComponent.Velocity;

	public override Vector2 HitBox => physicsComponent.HitBox;

	public override bool IsGrounded => physicsComponent.IsGrounded;

	public override Vector2 HitOffset => physicsComponent.HitOffset;

	public override int CurrentState => (int)stateMachineComponent.CurrentState;

	public override float CurrentNormalizedTime => stateMachineComponent.CurrentNormalizedTime;


    public override void Initialize(int ownerGuid, ENUM_ENTITY_TYPE type, bool isPlayer)
	{
		base.Initialize(ownerGuid, type, isPlayer);
		stateMachineComponent.Initialize(this);
	}

	public void SetEntityLayer(ENUM_LAYER_TYPE layerType)
	{
		renderingComponent.Initialize(layerType, Guid);
    }

    public override int GetSimulatedNextState(ICommand command)
    {
		var frameCommand = command.ToFrameCommand();
		var entity = frameCommand.ToEntity();
        return (int)stateMachineComponent.GetSimulatedNextState(frameCommand, (ENUM_CHARACTER_STATE)entity.entityState);
    }

	public override bool TryChangeState(ICommand command)
    {
		var frameCommand = command.ToFrameCommand();
		var nextState = GetSimulatedNextState(frameCommand);
		return stateMachineComponent.ChangeState((ENUM_CHARACTER_STATE)nextState, frameCommand);
	}

	public void AddMovement(Vector2 moveVec)
	{
		SetDirection(moveVec); // �Է� Ÿ�̹��� �� Flush Ÿ�̹��� �� ��� 
        physicsComponent.AddMovement(moveVec);
	}
	
	public void SetDirection(Vector2 moveVec)
	{
        renderingComponent.Look(moveVec.x < Mathf.Epsilon);
	}
}
