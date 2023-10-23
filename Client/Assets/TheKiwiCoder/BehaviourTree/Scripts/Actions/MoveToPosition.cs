using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using Unity.VisualScripting;

public class MoveToPosition : ActionNode
{
    protected override void OnStart() 
    {
        
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate() 
    {
        float distance = PhysicsSystem.Collision.GetXDistanceFromPlayer(context.entityComponent);
        var commandType = distance > Mathf.Epsilon ? ENUM_COMMAND_TYPE.RightMove : ENUM_COMMAND_TYPE.LeftMove;

        var command = MessageHelper.MakeCommand(commandType, context.entityComponent);
        context.entityComponent.SendCommand(command);

        return State.Success;
    }
}
