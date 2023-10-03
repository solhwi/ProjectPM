using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FrameSyncInputSnapShotMessage : NetworkMessage
{
	public int tickCount;

	public FrameInputSnapShotMessage[] snapshotMessages;
}

[System.Serializable]
public struct FrameEntityMessage : NetworkMessage
{
	public int entityGuid;
	public int entityState;

	public FrameEntityAnimationMessage animationMessage;

	public Vector2 myEntityPos;
	public Vector2 myEntityHitBox;
	public Vector2 myEntityOffset;
	public Vector2 myEntityVelocity;

	public int[] attackerEntities;

	public bool isGrounded;
}

[System.Serializable]
public struct FrameEntityAnimationMessage : NetworkMessage
{
	public int keyFrame;
	public float normalizedTime;
}

[System.Serializable]
public struct FrameInputSnapShotMessage : NetworkMessage
{
	public int ownerGuid;
	public int tickCount;

	public FrameEntityMessage[] entityMessages;

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