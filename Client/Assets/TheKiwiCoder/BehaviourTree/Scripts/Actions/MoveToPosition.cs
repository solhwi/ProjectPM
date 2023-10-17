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
        ENUM_COMMAND_TYPE commandType = distance > Mathf.Epsilon ? ENUM_COMMAND_TYPE.RightMove : ENUM_COMMAND_TYPE.LeftMove;

        var command = MessageHelper.MakeCommand(commandType);
        if (context.entityComponent.TryChangeState(command) == false)
            return State.Failure;

        return State.Success;
    }
}
