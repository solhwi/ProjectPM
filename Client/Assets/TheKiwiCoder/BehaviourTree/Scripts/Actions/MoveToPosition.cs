using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using Unity.VisualScripting;

public class MoveToPosition : ActionNode
{
    private float distance = 0.0f;

    protected override void OnStart() 
    {
        
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate() 
    {
        distance = EntityManager.Instance.GetXDistanceFromPlayer(context.entityComponent);
        var commandType = distance > Mathf.Epsilon ? ENUM_COMMAND_TYPE.RightMove : ENUM_COMMAND_TYPE.LeftMove;

        var command = MessageHelper.MakeCommand(commandType, context.entityComponent);
        context.entityComponent.TryChangeState(command);

        return State.Success;
    }
}
