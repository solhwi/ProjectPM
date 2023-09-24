using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AIAttribute("Dummy.prefab")]
public class DummyAIConrtollerComponent : AIControllerComponent
{
    public override ENUM_AI_TYPE AIType => ENUM_AI_TYPE.Dummy;    
}
