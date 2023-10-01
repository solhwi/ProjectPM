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

[RequireComponent(typeof(RenderingComponent))]
[RequireComponent(typeof(PhysicsComponent))]
[RequireComponent(typeof(CharacterStateMachineComponent))]
[Character("EntityMeditator.prefab")]
public class EntityMeditatorComponent : EntityComponent
{
	[SerializeField] private RenderingComponent renderingCompoonent = null;
	[SerializeField] private PhysicsComponent physicsComponent = null;
	[SerializeField] private CharacterStateMachineComponent stateMachineComponent = null;

	[SerializeField] private CharacterTransitionTable transitionTable = null;
	[SerializeField] private ConditionTable conditionTable = null;

    public override Vector2 Velocity => physicsComponent.Velocity;

	public override Vector2 HitBox => physicsComponent.HitBox;

	public override bool IsGrounded => physicsComponent.IsGrounded;

	public override Vector2 Offset => physicsComponent.hitOffset;

	public override void Initialize(int ownerGuid, ENUM_ENTITY_TYPE type)
	{
		base.Initialize(ownerGuid, type);
		stateMachineComponent.Initialize(this);
	}

	public void SetEntityLayer(ENUM_LAYER_TYPE layerType)
	{
		renderingCompoonent.Initialize(layerType, Guid);
    }

    public override ENUM_ENTITY_STATE GetSimulatedNextState(IStateMessage stateInfo)
    {
		var nextState = CurrentState;

        if (transitionTable.defaultTransitionList.Any())
        {
            var defaultTransition = transitionTable.defaultTransitionList.FirstOrDefault();
            var condition = conditionTable.GetCondition(defaultTransition.key);
            if (condition.IsSatisfied(stateInfo))
            {
                nextState = defaultTransition.nextState;
            }
        }

        if (transitionTable.loopTransitionDictionary.TryGetValue(CurrentState, out var loopTransition))
        {
            var condition = conditionTable.GetCondition(loopTransition.conditionType);
            if (condition.IsSatisfied(stateInfo))
            {
                nextState = CurrentState;
            }
        }

        foreach (var transition in transitionTable.transitionList)
        {
            if (transition.prevState == CurrentState)
            {
                var condition = conditionTable.GetCondition(transition.conditionType);
                if (condition.IsSatisfied(stateInfo))
                {
                    nextState = transition.nextState;
                }
            }
        }

		return nextState;
    }

	public override bool TryChangeState(IStateMessage stateInfo)
    {
		var entityFrameInfo = stateInfo.ConvertToEntity();

		CurrentState = entityFrameInfo.entityState > 0 ? (ENUM_ENTITY_STATE)entityFrameInfo.entityState : CurrentState;
        var nextState = GetSimulatedNextState(stateInfo);

		bool isChanged = CurrentState != nextState;
		if (isChanged)
		{
            stateMachineComponent.TryChangeState(nextState);
			CurrentState = nextState;
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
