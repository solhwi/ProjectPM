using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENUM_SKILL_TYPE
{
    None = 0,
    SlashPencil = 1,
    ThrowPencil = 2,
}

public class Skill
{
    protected AddressableResourceSystem resourceSystem;

    protected CharacterSkillTable skillTable;
    protected ENUM_SKILL_TYPE skillType;

    protected float maxCoolTime;
    protected float currentCoolTime;

    protected IEntity ownerEntity;
    protected ISkillTagAction skillTagAction;

    public Skill(ENUM_SKILL_TYPE type, CharacterSkillTable table, AddressableResourceSystem resourceSystem)
    {
        this.resourceSystem = resourceSystem;
        this.skillTable = table;
		this.skillType = type;

        var conditionInfo = skillTable.GetSkillConditionInfo(type);
        if (conditionInfo != null)
        {
            maxCoolTime = conditionInfo.cooltime;
		}

        skillTagAction = skillTable.GetSkillTagAction(type);
        currentCoolTime = 0.0f;
    }

    public void SetOwner(IEntity ownerEntity)
    {
        this.ownerEntity = ownerEntity;
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
        skillTagAction?.Trigger(resourceSystem);
    }
}