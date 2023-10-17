using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using System.Linq;

public class FindEnemy : ActionNode
{
    protected override void OnStart() 
    {
        
    }

    protected override void OnStop() 
    {

    }

    protected override State OnUpdate() 
    {
        blackboard.searchedEnemieDictionary.Clear();

        var hasSkillTypes = context.characterSkillTable.GetSkillTypes(context.entityComponent.EntityType);
        if (hasSkillTypes == null)
            return State.Success;

        foreach(var skillType in hasSkillTypes)
        {
            var enemies = EntityManager.Instance.GetSearchedEntities(context.entityComponent, skillType);
            if (enemies == null || enemies.Any() == false)
                continue;

            blackboard.searchedEnemieDictionary.Add(skillType, enemies);
        }

        return State.Success;
    }
}
