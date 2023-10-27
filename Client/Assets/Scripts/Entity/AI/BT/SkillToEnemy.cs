using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using System.Linq;
using System.Xml;

public class SkillToEnemy : ActionNode
{
    ENUM_SKILL_TYPE skillType = ENUM_SKILL_TYPE.None;

    protected override void OnStart()
    {
		var validSkillTypes = blackboard.searchedEnemieDictionary.Keys;
		skillType = GetBiggestManaSkill(validSkillTypes);    
	}

    protected override void OnStop()
    {

    }

    private ENUM_SKILL_TYPE GetBiggestManaSkill(IEnumerable<ENUM_SKILL_TYPE> skillTypes)
    {
		var characterSkillTable = context.scriptParsingSystem.GetTable<CharacterSkillTable>();
		if (characterSkillTable == null)
			return ENUM_SKILL_TYPE.None;

		return skillTypes
            .Where(characterSkillTable.IsUseMana) // ������ ���� ��ų �߿�
            .OrderByDescending(characterSkillTable.GetUseMana) // ���� ū ��
            .FirstOrDefault();
    }

    protected override State OnUpdate()
    {
        if (skillType == ENUM_SKILL_TYPE.None)
            return State.Failure;

		var command = context.commandSystem.MakeCommand(ENUM_COMMAND_TYPE.Skill, context.entityComponent);
        context.entityComponent.PushCommand(command);

        return State.Success;
    }
}
