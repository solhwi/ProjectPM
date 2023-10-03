using StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// 이 곳에 타입이 추가되면 Unity Layer에도 추가해야 합니다.
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

// 앤티티가 이 컴포넌트를 갖도록 한다.
// 앤티티 매니저 - 앤티티 - 앤티티 컴포넌트
// 관계를 갖고, 앤티티 매니저가 인풋과 현재 앤티티의 상태로 위치를 결정한다.
// 위치를 기반으로 충돌을 결정한다.
// 인풋, 앤티티, 충돌 + 애니메이션 key frame 으로 다음 상태를 결정한다.

// component는 entity를 기반으로 view를 반영한다.

// 이게 정석같기는 한데...
// 1. 그럼 충돌에 대한 부분을 직접 구현해야 한다.
// 2. 애니메이션도 직접 구현해서 데이터 레벨로 내려야 한다.

// 분리는 안하기로 결정...
// Entity 데이터를 분리하는 게 아니라 EntityComponent를 상속받는 형식으로 간다.
// Entity엔 사용할 데이터 형식을 정의한다.

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

	public void MakeInvincible(bool isEnable)
	{
		damageableComponent.isEnable = isEnable;
	}

	public void MakeSuperArmor(bool isEnable)
	{

	}
}
