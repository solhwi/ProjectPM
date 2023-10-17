using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using System.Linq;

public class AttackToEnemy : ActionNode
{
    protected override void OnStart() 
    {
        
    }

    protected override void OnStop() 
    {

    }

    private bool IsAttack(ENUM_SKILL_TYPE skillType)
    {
        return context.characterSkillTable.IsUseMana(skillType) == false;
    }

    protected override State OnUpdate() 
    {
        var validSkillTypes = blackboard.searchedEnemieDictionary.Keys.ToList();
        var currentSkill = validSkillTypes.FirstOrDefault(IsAttack);
        if (currentSkill == ENUM_SKILL_TYPE.None)
            return State.Failure;

        var command = MessageHelper.MakeCommand(ENUM_COMMAND_TYPE.Attack, context.entityComponent);
        context.entityComponent.TryChangeState(command);

        return State.Success;
    }
}
