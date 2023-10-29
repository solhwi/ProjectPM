using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using System.Linq;

public class FindEnemy : ActionNode
{
    CharacterSkillTable skillTable = null;
	List<ENUM_SKILL_TYPE> hasSkillTypes = new List<ENUM_SKILL_TYPE>();

    protected override void OnStart() 
    {
		skillTable = context.scriptParsingSystem.GetTable<CharacterSkillTable>();
        if (skillTable == null)
            return;

		hasSkillTypes = skillTable.GetSkillTypes(context.entityComponent.EntityType).ToList();
	}

	protected override void OnStop() 
    {

    }

    protected override State OnUpdate() 
    {
        blackboard.searchedEnemieDictionary.Clear();

		foreach (var skillType in hasSkillTypes)
        {
            var skillInfo = skillTable.GetSkillConditionInfo(skillType);
            if (skillInfo == null)
                continue;

			Vector2 searchBox = new(skillInfo.searchBoxX, skillInfo.searchBoxY);
			Vector2 searchOffset = new(skillInfo.searchOffsetX, skillInfo.searchOffsetY);

			var enemies = context.entitySystem.GetOverlapEnemies(context.entityComponent, searchBox, searchOffset);
            if (enemies == null || enemies.Any() == false)
                continue;

            blackboard.searchedEnemieDictionary.Add(skillType, enemies);
        }

        return State.Success;
    }
}
