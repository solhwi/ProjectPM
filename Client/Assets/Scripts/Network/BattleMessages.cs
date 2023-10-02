using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FrameSyncSnapShotMessage : NetworkMessage
{
	public int tickCount;

	public FrameEntityMessage[] entityMessages;
}

[System.Serializable]
public struct FrameEntityMessage : NetworkMessage, IStateMessage
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
public struct FrameEntityAnimationMessage : NetworkMessage, IStateMessage
{
	public int keyFrame;
	public float normalizedTime;
}

[System.Serializable]
public struct FrameInputSnapShotMessage : NetworkMessage, IStateMessage
{
	public int ownerGuid;
	public int tickCount;

	public FrameEntityMessage[] entityMessages;

	public FrameEntityMessage playerEntityMessage;
	public FrameInputMessage playerInputMessage;
}

[System.Serializable]
public struct FrameInputMessage : NetworkMessage, IStateMessage
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