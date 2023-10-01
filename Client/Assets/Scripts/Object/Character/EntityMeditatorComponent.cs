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
