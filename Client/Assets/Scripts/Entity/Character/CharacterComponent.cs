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
	Idle = 1, // 멈춤
	Move, // 좌, 우
	JumpUp, // 점프 (하면서 물리적인 이동이 가능)
	JumpDown, // 점프 다운
	Dash, // 대쉬
	Down, // 다운
	Landing, // 착지
	Recovery, // 기상
	StandHit, // 데미지 입음
	AirborneHit, // 공중에서 데미지 입음
	Die, // 사망

	Attack1, // 공격
	Attack2,
	Attack3,
	DashAttack, // 대쉬 공격
	JumpAttack, // 점프 공격

	Skill, // 스킬 공격
	DashSkill, // 대쉬 스킬
	JumpSkill, // 점프 스킬

	Guard, // 가드
	Ultimate, // 궁극기
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

    public override Vector2 Velocity => physicsComponent.Velocity;

	public override Vector2 HitBox => physicsComponent.HitBox;

	public override bool IsGrounded => physicsComponent.IsGrounded;

	public override Vector2 Offset => physicsComponent.HitOffset;

	public override int CurrentState => (int)stateMachineComponent.CurrentState;

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
		physicsComponent.AddMovement(moveVec);
	}
}
