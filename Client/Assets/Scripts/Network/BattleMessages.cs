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

public class MessageHelper
{
	public static FrameSnapShotMessage MakeSnapShot(int ownerGuid, int tickCount)
	{
		var newSnapShot = new FrameSnapShotMessage();

		var entities = EntityManager.Instance.GetEntities(ownerGuid);
		if (entities == null || entities.Any() == false)
			return default;

		newSnapShot.entityMessages = entities.Select(MakeEntityMessage).ToArray();

		var playerEntity = EntityManager.Instance.PlayerEntity;
		if (playerEntity == null)
			return default;

		newSnapShot.commandMessage.playerEntityMessage = MakeEntityMessage(playerEntity);
		newSnapShot.commandMessage.playerInputMessage = MakeInputMessage();
		
		newSnapShot.tickCount = tickCount;
		return newSnapShot;
	}
	
	public static FrameInputMessage MakeInputMessage()
	{
		return InputManager.Instance.FlushInput(Time.frameCount);
	}

    private static FrameInputMessage MakeFrameMessageByCommand(ENUM_COMMAND_TYPE commandType)
    {
        var message = new FrameInputMessage();

        switch (commandType)
        {
            case ENUM_COMMAND_TYPE.LeftMove:
                message.moveInput = Vector2.left;
                break;

            case ENUM_COMMAND_TYPE.RightMove:
                message.moveInput = Vector2.right;
                break;

            case ENUM_COMMAND_TYPE.LeftJump:
                message.moveInput = new Vector2(-1, 1);
                break;

            case ENUM_COMMAND_TYPE.RightJump:
                message.moveInput = new Vector2(1, 1);
                break;

            case ENUM_COMMAND_TYPE.LeftDash:
                message.moveInput = Vector2.left;
                message.isDash = true;
                break;

            case ENUM_COMMAND_TYPE.RightDash:
                message.moveInput = Vector2.right;
                message.isDash = true;
                break;

            case ENUM_COMMAND_TYPE.Attack:
                message.pressedAttackKeyNum = (int)ENUM_ATTACK_KEY.ATTACK;
                break;

            case ENUM_COMMAND_TYPE.Skill:
                message.pressedAttackKeyNum = (int)ENUM_ATTACK_KEY.SKILL;
                break;

            case ENUM_COMMAND_TYPE.Ultimate:
                message.pressedAttackKeyNum = (int)ENUM_ATTACK_KEY.ULTIMATE;
                break;

            case ENUM_COMMAND_TYPE.Guard:
                message.isGuard = true;
                break;
        }

        return message;

    }

    public static FrameCommandMessage MakeCommand()
	{
		var message = new FrameCommandMessage();
		message.playerInputMessage = MakeInputMessage();

		var playerEntity = EntityManager.Instance.PlayerEntity;
		if (playerEntity == null)
			return default;

		message.playerEntityMessage = MakeEntityMessage(playerEntity);
		return message;
	}

	public static FrameCommandMessage MakeCommand(ENUM_COMMAND_TYPE commandType, EntityComponent ownerEntity)
	{
        var message = new FrameCommandMessage();
        message.playerInputMessage = MakeFrameMessageByCommand(commandType);
        message.playerEntityMessage = MakeEntityMessage(ownerEntity);
        return message;
    }

	public static FrameEntityMessage MakeEntityMessage(EntityComponent entity)
	{
		var playerEntityMessage = new FrameEntityMessage();

		playerEntityMessage.pos = entity.Position;
		playerEntityMessage.entityGuid = entity.Guid;
		playerEntityMessage.velocity = entity.Velocity;
		playerEntityMessage.hitbox = entity.HitBox;
		playerEntityMessage.offset = entity.HitOffset;
		playerEntityMessage.entityState = entity.CurrentState;
		playerEntityMessage.normalizedTime = entity.CurrentNormalizedTime;
		playerEntityMessage.isGrounded = entity.IsGrounded;

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