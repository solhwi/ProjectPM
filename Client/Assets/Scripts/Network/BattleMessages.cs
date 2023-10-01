using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FrameSyncSnapShotMessage : NetworkMessage
{
	public int ownerGuid;
	public int tickCount;

	public FrameEntityMessage[] entityMessages;
}

public struct FrameEntityMessage : NetworkMessage, IStateInfo
{
	public int entityGuid;
	public int entityCurrentState;

	public Vector2 myEntityPos;
	public Vector2 myEntityHitBox;
	public Vector2 myEntityOffset;
	public Vector2 myEntityVelocity;

	public int[] attackerEntities;

	public bool isGrounded;
}

public struct FrameSnapShotMessage : NetworkMessage, IStateInfo
{
	public int ownerGuid;
	public int tickCount;

	public FrameEntityMessage[] entityMessages;
	public FrameSyncInputInfo inputInfo;
}