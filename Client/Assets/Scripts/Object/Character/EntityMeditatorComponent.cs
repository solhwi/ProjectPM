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
	Idle, // 멈춤
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

    private CharacterTransitionTable transitionTable = null;
    private ConditionTable conditionTable = null;

    // 현재 스테이트 정보를 가지고 있어야 한다.
    // 그래야 ObjectManager가 충돌, 스테이트 처리가 가능하다.
    private FrameSyncStateParam stateParam = new();

    public override Vector2 Velocity => physicsComponent.Velocity;

	public override Vector2 HitBox => physicsComponent.HitBox;

	public override bool IsGrounded => physicsComponent.IsGrounded;

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

    public override ENUM_ENTITY_STATE GetSimulatedNextState(IStateInfo stateInfo)
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

    public override bool TryChangeState(IStateInfo stateInfo)
    {
        var nextState = GetSimulatedNextState(stateInfo);

		bool isChanged = CurrentState != nextState;
		if (isChanged)
		{
            stateMachineComponent.TryChangeState(nextState, stateInfo);
			CurrentState = nextState;
        }

        return isChanged;
	}

    public void OnPostMove(Vector2 moveVec)
	{
		physicsComponent.Move(moveVec);
	}
}
