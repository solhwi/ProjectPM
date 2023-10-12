using JetBrains.Annotations;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MessageHelper
{
	public static FrameSnapShotMessage MakeSnapShot(int ownerGuid, int tickCount)
	{
		var newSnapShot = new FrameSnapShotMessage();
		newSnapShot.commandMessage.playerInputMessage = InputManager.Instance.FlushInput();

		var playerEntity = EntityManager.Instance.PlayerEntity;
		if (playerEntity == null)
			return default;

		newSnapShot.commandMessage.playerEntityMessage = MakeEntityMessage(playerEntity);

		var entities = EntityManager.Instance.GetEntities(ownerGuid);
		if (entities == null || entities.Any() == false)
			return default;

		newSnapShot.entityMessages = entities.Select(MakeEntityMessage).ToArray();
		newSnapShot.tickCount = tickCount;
		return newSnapShot;
	}

	public static FrameCommandMessage MakeCommand()
	{
		var message = new FrameCommandMessage();
		message.playerInputMessage = InputManager.Instance.FlushInput();

		var playerEntity = EntityManager.Instance.PlayerEntity;
		if (playerEntity == null)
			return default;

		message.playerEntityMessage = MakeEntityMessage(playerEntity);
		return message;
	}

	public static FrameEntityMessage MakeEntityMessage(EntityComponent entity)
	{
		var playerEntityMessage = new FrameEntityMessage();

		playerEntityMessage.pos = entity.Position;
		playerEntityMessage.entityGuid = entity.Guid;
		playerEntityMessage.velocity = entity.Velocity;
		playerEntityMessage.hitbox = entity.HitBox;
		playerEntityMessage.offset = entity.Offset;
		playerEntityMessage.entityState = entity.CurrentState;

		return playerEntityMessage;
	}
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