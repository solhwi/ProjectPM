using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AIAttribute("NormalMonsterAI.prefab")]
public class NormalMonsterAIConrtollerComponent : AIControllerComponent
{
    public override ENUM_AI_TYPE AIType => ENUM_AI_TYPE.NormalMonster;    
}
