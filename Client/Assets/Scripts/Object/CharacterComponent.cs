using StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static CharacterStatTable;

public enum ENUM_CHARACTER_TYPE
{
    RedMan = 0,
	BlueMan = 1,
	GreenMan = 2,
}

[Serializable]
public enum CharacterState
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
	public FrameSyncInputData userInput;

	public bool IsGrounded;
	public Vector2 Velocity;

    public IEnumerable<AttackableComponent> attackers;
    public ObjectComponent defender;

	public void Clear()
	{
		userInput = default;
		IsGrounded = false;
		Velocity = default;
		attackers = null;
		defender = null;
	}
}

[RequireComponent(typeof(PhysicsComponent))]
[RequireComponent(typeof(CharacterStateMachineComponent))]
public abstract class CharacterComponent : ObjectComponent
{
	public abstract ENUM_CHARACTER_TYPE CharacterType
	{
		get;
	}

	// 아래 정보들은 ScriptableObject로 빼자.

	public bool IsBoss { get; private set; } = false;

	private PhysicsComponent physicsComponent = null;
	private CharacterStateMachineComponent stateMachineComponent = null;

	private FrameSyncStateParam stateParam = new();

	public override int Initialize(ENUM_TEAM_TYPE teamType, bool isBoss)
	{
		IsBoss = isBoss;

		physicsComponent = GetComponent<PhysicsComponent>();

		stateMachineComponent = GetComponent<CharacterStateMachineComponent>();
		stateMachineComponent.Initialize(this);

		return base.Initialize(teamType, isBoss);
    }

    public void OnPlayerInput(FrameSyncInputData frameData)
	{
		stateParam.Clear();
		stateParam.userInput = frameData;
    }

	public override void OnOtherInput(IEnumerable<AttackableComponent> attackers)
	{
		if (attackers.Any() == false)
			return;

        stateParam.attackers = attackers;
		stateParam.defender = this;
    }

	public override void OnPostInput()
	{
        stateParam.Velocity = physicsComponent.Velocity;
		stateParam.IsGrounded = physicsComponent.IsGrounded;

		stateMachineComponent.TryChangeState(stateParam);
	}

	public void OnPostMove(Vector2 moveVec)
	{
		physicsComponent.Move(moveVec);
	}

	public override void OnUpdateAnimation()
	{
		// 반영된 물리 정보, 애니메이션의  정보로 
		// ScriptableObject 데이터를 반영한다.
	}
}
