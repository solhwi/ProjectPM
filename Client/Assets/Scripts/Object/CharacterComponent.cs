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
    Normal = 0,
}

public enum CharacterState
{
	Idle, // 멈춤
	Move, // 좌, 우
	Jump, // 점프 (하면서 물리적인 이동이 가능)
	JumpDown, // 점프 다운
	Dash, // 대쉬
	Down, // 다운
	Landing, // 착지
	Recovery, // 기상
	Hit, // 데미지 입음

	Attack, // 공격
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

public interface IStateInput
{

}

public struct FrameSyncCharacterStateInput : IStateInput
{
	public FrameSyncInputData frameData;

	public bool IsGrounded;
	public Vector2 Velocity;

    public IEnumerable<AttackableComponent> attackers;
    public ObjectComponent defender;
}

[RequireComponent(typeof(PhysicsComponent))]
[RequireComponent(typeof(CharacterAnimatorComponent))]
public abstract class CharacterComponent : ObjectComponent
{
	public abstract ENUM_CHARACTER_TYPE CharacterType
	{
		get;
	}

	private PhysicsComponent physicsComponent = null;
	private CharacterAnimatorComponent animatorComponent = null;

	private FrameSyncCharacterStateInput inputData = new();

	public override void Initialize(ENUM_TEAM_TYPE teamType, bool isBoss)
	{
		base.Initialize(teamType, isBoss);

		physicsComponent = GetComponent<PhysicsComponent>();

		animatorComponent = GetComponent<CharacterAnimatorComponent>();
		animatorComponent.Initialize(this);
	}

	public void OnPlayerInput(FrameSyncInputData frameData)
	{
		inputData = new();
        inputData.frameData = frameData;
    }

	public override void OnOtherInput(IEnumerable<AttackableComponent> attackers)
	{
		if (attackers.Any() == false)
			return;

        inputData.attackers = attackers;
		inputData.defender = this;
    }

	public override void OnPostInput()
	{
        inputData.Velocity = physicsComponent.Velocity;
		inputData.IsGrounded = physicsComponent.IsGrounded;

		animatorComponent.TryChangeState(inputData);
	}

	public void OnPostMove(Vector2 moveVec)
	{
		physicsComponent.Move(moveVec);
	}

	public void OnChangeDamageBox(Vector2 damageBox)
	{
		physicsComponent.SetCollisionBox(damageBox);
	}

	public override void OnUpdateAnimation()
	{

	}
}
