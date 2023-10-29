using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommandSystem : MonoSystem
{
    [SerializeField] private FrameInputSystem inputSystem = null;
    [SerializeField] private EntitySystem entitySystem = null;

    protected override void OnReset()
    {
        base.OnReset();

        inputSystem = SystemHelper.GetSystemAsset<FrameInputSystem>();
        entitySystem = SystemHelper.GetSystemAsset<EntitySystem>();
    }

    public FrameSnapShotMessage MakeSnapShot(int ownerGuid, int tickCount)
    {
        var newSnapShot = new FrameSnapShotMessage();

        var entities = entitySystem.GetEntities(ownerGuid);
        if (entities == null || entities.Any() == false)
            return default;

        newSnapShot.entityMessages = entities.Select(MakeEntityMessage).ToArray();

        var playerEntity = entitySystem.Player;
        if (playerEntity == null)
            return default;

        newSnapShot.commandMessage.playerEntityMessage = MakeEntityMessage(playerEntity);
        newSnapShot.commandMessage.playerInputMessage = MakeInputMessage();

        newSnapShot.tickCount = tickCount;
        return newSnapShot;
    }

    private FrameInputMessage MakeInputMessage()
    {
        return inputSystem.FlushInput(Time.frameCount);
    }

    private FrameInputMessage MakeFrameMessageByCommand(ENUM_COMMAND_TYPE commandType)
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

    public FrameCommandMessage MakeCommand()
    {
        var message = new FrameCommandMessage();
        message.playerInputMessage = MakeInputMessage();

        var playerEntity = entitySystem.Player;
        if (playerEntity == null)
            return default;

        message.playerEntityMessage = MakeEntityMessage(playerEntity).AddAttackerEntities(entitySystem);
        return message;
    }

    public FrameCommandMessage MakeCommand(ENUM_COMMAND_TYPE commandType, IEntity entity)
    {
        var message = new FrameCommandMessage();
        message.playerInputMessage = MakeFrameMessageByCommand(commandType);
        message.playerEntityMessage = MakeEntityMessage(entity).AddAttackerEntities(entitySystem);
        return message;
    }

	private FrameEntityMessage MakeEntityMessage(IEntity entity)
    {
        var playerEntityMessage = new FrameEntityMessage();

        playerEntityMessage.pos = entity.Position;
        playerEntityMessage.entityGuid = entity.EntityGuid;
        playerEntityMessage.velocity = entity.Velocity;
        playerEntityMessage.hitbox = entity.HitBox;
        playerEntityMessage.offset = entity.HitOffset;
        playerEntityMessage.entityState = entity.CurrentState;
        playerEntityMessage.normalizedTime = entity.CurrentStateNormalizedTime;
        playerEntityMessage.isGrounded = entity.IsGrounded;

        return playerEntityMessage;
    }
}
