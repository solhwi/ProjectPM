using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using System.Linq;

public class FindEnemy : ActionNode
{
    List<ENUM_SKILL_TYPE> hasSkillTypes = new List<ENUM_SKILL_TYPE>();

    protected override void OnStart() 
    {
		var characterSkillTable = context.scriptParsingSystem.GetTable<CharacterSkillTable>();
        if (characterSkillTable == null)
            return;

		hasSkillTypes = characterSkillTable.GetSkillTypes(context.entityComponent.EntityType).ToList();
	}

	protected override void OnStop() 
    {

    }

    protected override State OnUpdate() 
    {
        blackboard.searchedEnemieDictionary.Clear();

        foreach (var skillType in hasSkillTypes)
        {
            var enemies = context.entitySystem.GetSearchedEntities(context.entityComponent, skillType);
            if (enemies == null || enemies.Any() == false)
                continue;

            blackboard.searchedEnemieDictionary.Add(skillType, enemies);
        }

        return State.Success;
    }
}
