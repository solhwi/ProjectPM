using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ControllerManager : Singleton<ControllerManager>
{
    public PlayerInput SetPlayerController(ObjectComponent targetObject)
    {
        return targetObject.GetOrAddComponent<PlayerInput>();
    }
}