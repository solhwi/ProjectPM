using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using System.Linq;

public class AttackToEnemy : ActionNode
{
	ENUM_SKILL_TYPE skillType = ENUM_SKILL_TYPE.None;

	protected override void OnStart() 
    {
		var validSkillTypes = blackboard.searchedEnemieDictionary.Keys;
		skillType = validSkillTypes.FirstOrDefault(IsAttack);
	}

    protected override void OnStop() 
    {

    }

    private bool IsAttack(ENUM_SKILL_TYPE skillType)
    {
		var characterSkillTable = context.scriptParsingSystem.GetTable<CharacterSkillTable>();
		if (characterSkillTable == null)
			return false;

		return characterSkillTable.IsUseManaSkill(skillType) == false;
    }

    protected override State OnUpdate() 
    {
        if (skillType == ENUM_SKILL_TYPE.None)
            return State.Failure;

        var command = context.commandSystem.MakeCommand(ENUM_COMMAND_TYPE.Attack, context.entityComponent);
        context.entityComponent.PushCommand(command);

        return State.Success;
    }
}
