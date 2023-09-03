using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum ENUM_CHARACTER_TYPE
{
    Normal = 0,
}

public enum CharacterState
{
	Idle, // 멈춤
	Move, // 좌, 우
	Jump, // 점프 (하면서 물리적인 이동이 가능)
	Dash, // 대쉬
	Down, // 다운
	Landing, // 착지
	Recovery, // 기상
	Hit, // 데미지 입음

	Attack, // 공격
	DashAttack, // 대쉬 공격
	JumpAttack, // 점프 공격

	Skill, // 스킬 공격
	DashSkill, // 대쉬 스킬
	JumpSkill, // 점프 스킬

	Guard, // 가드
	Ultimate, // 궁극기
}

public interface IStateParam
{

}


/// <summary>
/// 인풋 > 인풋에 자신의 데이터를 더함 > FSM에 넘김 > FSM이 조합한 결과물로 데이터를 수정함 > 레이트 업데이트에서 뷰를 수정함
/// 
/// 
/// </summary>
public class FrameSyncCharacterInputData : IStateParam
{
	public FrameSyncInputData frameData = null;

	public bool IsGrounded = false;
	public Vector2 Velocity = default;

    public IEnumerable<AttackableComponent> attackers = null;
    public ObjectComponent defender = null;
}

public class FrameSyncCharacterOutputData : IStateParam
{
	public readonly Vector2 moveVec = default;

	public FrameSyncCharacterOutputData()
	{
		moveVec = default;
    }

	public FrameSyncCharacterOutputData(Vector2 moveVec)
	{
		this.moveVec = moveVec;
	}
}


[RequireComponent(typeof(PhysicsComponent))]
[RequireComponent(typeof(CharacterAnimatorComponent))]
public class CharacterComponent : ObjectComponent
{
    [SerializeField] private CharacterStatTable characterStatTable = null;
	[SerializeField] private PhysicsComponent physicsComponent = null;
	[SerializeField] private CharacterAnimatorComponent animatorComponent = null;
	[SerializeField] private ENUM_CHARACTER_TYPE characterType;

	private FrameSyncCharacterInputData inputData = new();
    private FrameSyncCharacterOutputData outputData = new();

	public void OnPlayerInput(FrameSyncInputData frameData)
	{
		inputData ??= new();
        inputData.frameData = frameData;
    }

	public override void OnOtherInput(IEnumerable<AttackableComponent> attackers)
	{
		if (attackers.Any() == false)
			return;

        inputData ??= new();
        inputData.attackers = attackers;
		inputData.defender = this;
    }

	public override void OnPostInput()
	{
        inputData ??= new();
        inputData.Velocity = physicsComponent.Velocity;
		inputData.IsGrounded = physicsComponent.IsGrounded;

		animatorComponent.TryChangeState(inputData);
    }

	public void OnPostStateUpdate(FrameSyncCharacterOutputData output)
	{
		outputData = output;
    }

	// 물리 처리를 시도한다.
	public override void OnUpdateAnimation()
	{
		if (outputData == null)
			return;

		physicsComponent.Move(outputData.moveVec);
		outputData = null;

    }
}
