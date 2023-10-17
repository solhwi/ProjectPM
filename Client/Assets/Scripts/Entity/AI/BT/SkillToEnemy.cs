using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using System.Linq;
using System.Xml;

public class SkillToEnemy : ActionNode
{
    private CharacterSkillTable skillTable = null;
    private ENUM_SKILL_TYPE currentSkill = ENUM_SKILL_TYPE.None;

    protected override void OnStart()
    {
        skillTable = ScriptParserManager.Instance.GetTable<CharacterSkillTable>();
        var skillTypes = blackboard.searchedEnemieDictionary.Keys.ToList();
        skillTypes = skillTypes.OrderByDescending(skillTable.GetUseMana).ToList();
        currentSkill = skillTypes.FirstOrDefault(skillTable.IsUseMana);
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        if (currentSkill != ENUM_SKILL_TYPE.None)
            return State.Failure;

        var command = MessageHelper.MakeCommand(ENUM_COMMAND_TYPE.Skill);
        if (context.entityComponent.TryChangeState(command) == false)
            return State.Failure;

        return State.Success;
    }
}
