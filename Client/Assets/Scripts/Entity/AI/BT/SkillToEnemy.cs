using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using System.Linq;
using System.Xml;

public class SkillToEnemy : ActionNode
{

    protected override void OnStart() 
    {

    }

    protected override void OnStop() 
    {

    }

    protected override State OnUpdate()
    {        
        // 거리별로 공격이 성공할 수 있는 상대를 선정
        // 공격이 성공할 수 있는 상대가 존재한다면
        // 공격 실행 후 성공
        return State.Success;
    }
}
