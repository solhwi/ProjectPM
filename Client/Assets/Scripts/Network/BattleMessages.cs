using JetBrains.Annotations;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public enum ENUM_COMMAND_TYPE
{
	LeftMove,
	RightMove,
	LeftJump,
	RightJump,
    LeftDash,
    RightDash,
    Attack,
	DashAttack,
	JumpAttack,
	Skill,
	DashSkill,
	JumpSkill,
	Ultimate,
    Guard,
}

[System.Serializable]
public struct FrameSyncSnapShotMessage : NetworkMessage
{
	public int tickCount;

	public FrameSnapShotMessage[] snapshotMessages;
}

[System.Serializable]
public struct FrameEntityMessage : NetworkMessage
{
	public int entityGuid;
	public int entityState;

	public Vector2 pos;
	public Vector2 hitbox;
	public Vector2 offset;
	public Vector2 velocity;

	public int[] overlappedEntities;

	public bool isGrounded;
	public float normalizedTime;
}

[System.Serializable]
public struct FrameSnapShotMessage : NetworkMessage
{
	public int ownerGuid;
	public int tickCount;

	public FrameEntityMessage[] entityMessages;
	public FrameCommandMessage commandMessage;
}

[System.Serializable]
public struct FrameCommandMessage : NetworkMessage, ICommand
{
	public FrameEntityMessage playerEntityMessage;
	public FrameInputMessage playerInputMessage;
}

[System.Serializable]
public struct FrameInputMessage : NetworkMessage
{
	public Vector2 moveInput;
	public int pressedAttackKeyNum;
	public bool isDash;
	public bool isGuard;
	public int frameCount;

	public FrameInputMessage(Vector2 moveInput, ENUM_ATTACK_KEY pressedAttackKey, bool isDash, bool isGuard, int frameCount)
	{
		this.moveInput = moveInput;
		this.pressedAttackKeyNum = (int)pressedAttackKey;
		this.isDash = isDash;
		this.isGuard = isGuard;
		this.frameCount = frameCount;
	}
}