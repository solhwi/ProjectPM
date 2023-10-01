using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ControllerManager : Singleton<ControllerManager>
{
    public PlayerInputComponent SetPlayerController(EntityComponent targetObject)
    {
        return targetObject.GetOrAddComponent<PlayerInputComponent>();
    }
}
