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

    private ENUM_SKILL_TYPE GetBiggestManaSkill(IEnumerable<ENUM_SKILL_TYPE> skillTypes)
    {
        return skillTypes
            .Where(context.characterSkillTable.IsUseMana) // 마나를 쓰는 스킬 중에
            .OrderByDescending(context.characterSkillTable.GetUseMana) // 제일 큰 것
            .FirstOrDefault();
    }

    protected override State OnUpdate()
    {
        var validSkillTypes = blackboard.searchedEnemieDictionary.Keys.ToList();
        var currentSkill = GetBiggestManaSkill(validSkillTypes);
        if (currentSkill == ENUM_SKILL_TYPE.None)
            return State.Failure;

        var command = MessageHelper.MakeCommand(ENUM_COMMAND_TYPE.Skill, context.entityComponent);
        context.entityComponent.TryChangeState(command);

        return State.Success;
    }
}
