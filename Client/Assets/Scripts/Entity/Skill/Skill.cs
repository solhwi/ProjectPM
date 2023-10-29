using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENUM_SKILL_TYPE
{
    None = 0,
    SlashPencil = 1,
    ThrowPencil = 2,
}

public abstract class Skill
{
    private CharacterSkillTable skillTable;
	private ENUM_SKILL_TYPE skillType;

	private float maxCoolTime;
    private float currentCoolTime;

    public Skill(ENUM_SKILL_TYPE type, CharacterSkillTable table)
    {
        this.skillTable = table;
		this.skillType = type;

        var conditionInfo = skillTable.GetSkillConditionInfo(type);
        if (conditionInfo != null)
        {
            maxCoolTime = conditionInfo.cooltime;
		}

        currentCoolTime = 0.0f;
    }

    private bool IsCoolTime()
    {
        return maxCoolTime > currentCoolTime;
    }

    public virtual bool IsSatisfied()
    {
        return IsCoolTime() == false;
    }

    public virtual void OnUpdate(float deltaTime)
    {
        currentCoolTime += deltaTime;
    }

    public virtual void Trigger()
    {
        currentCoolTime = default;
    }
}

public class ThrowPencil : Skill
{
	public ThrowPencil(ENUM_SKILL_TYPE type, CharacterSkillTable table) : base(type, table)
	{
	}
}

public class SlashPencil : Skill
{
	public SlashPencil(ENUM_SKILL_TYPE type, CharacterSkillTable table) : base(type, table)
	{
	}
}
