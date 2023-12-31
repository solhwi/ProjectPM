using Cysharp.Threading.Tasks;
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
public class CharacterBehaviour : EntityBehaviour
{
	[SerializeField] private RenderingComponent renderingComponent = null;
	[SerializeField] private PhysicsComponent physicsComponent = null;
	[SerializeField] private CharacterStateMachineComponent stateMachineComponent = null;
	[SerializeField] private DamageableComponent damageableComponent = null;	
	[SerializeField] private CharacterStatTable statTable = null;

	public float JumpPower => statTable.GetStat(EntityType)?.jumpPower ?? 0.0f;

	public override bool IsAttackable => isAttackable;

	public override bool IsLeftDirection => renderingComponent.IsLeftDirection;

	public override Vector2 Velocity => physicsComponent.Velocity;

	public override Vector2 HitBox => physicsComponent.HitBox;

	public override bool IsGrounded => physicsComponent.CheckGrounded();

	public override Vector2 HitOffset => physicsComponent.HitOffset;

	public override int CurrentState => (int)stateMachineComponent.CurrentState;

	public override float CurrentStateNormalizedTime => stateMachineComponent.CurrentStateNormalizedTime;

	private bool isAttackable = false;

	protected override void Reset()
	{
		base.Reset();
		renderingComponent = GetComponent<RenderingComponent>();
		physicsComponent = GetComponent<PhysicsComponent>();
		stateMachineComponent = GetComponent<CharacterStateMachineComponent>();
		damageableComponent = GetComponent<DamageableComponent>();
		statTable = AssetLoadHelper.GetDataAsset<CharacterStatTable>();
	}


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

    protected int GetSimulatedNextState(ICommand command)
    {
		var frameMessage = command.ToFrameMessage();
		var entity = frameMessage.ToEntity();
        return (int)stateMachineComponent.GetSimulatedNextState(frameMessage, (ENUM_CHARACTER_STATE)entity.entityState);
    }

	public override void PushCommand(ICommand command)
    {
		var frameMessage = command.ToFrameMessage();
        var nextState = GetSimulatedNextState(frameMessage);

        stateMachineComponent.PushCommand(frameMessage);
        stateMachineComponent.ChangeState((ENUM_CHARACTER_STATE)nextState);
	}

	public override bool IsDamageable(IEntity attackerEntity)
	{
		return damageableComponent.IsDamageable(attackerEntity);
	}

	public void AddMovement(Vector2 moveVec)
	{
        physicsComponent.AddMovement(moveVec);
		SetDirection(physicsComponent.Velocity); // 누적된 이동량으로 체크합니다.
	}
	
	public void SetDirection(Vector2 moveVec)
	{
		if (Mathf.Approximately(moveVec.x, 0.0f))
			return;

		bool isLeft = moveVec.x < Mathf.Epsilon;
		renderingComponent.Look(isLeft);
	}

	public void SetInvincible(bool able)
	{
		damageableComponent.Invincible = able;
	}

	public void SetAttackable(bool able)
	{
		isAttackable = able;
	}
}
