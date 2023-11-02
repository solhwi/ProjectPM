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
    protected readonly AddressableResourceSystem resourceSystem;

    protected readonly CharacterSkillTable skillTable;
    public readonly ENUM_SKILL_TYPE skillType;
    public readonly ENUM_CHARACTER_STATE characterState;
    protected readonly ISkillTagAction skillTagAction;

    protected readonly float maxCoolTime;
	protected readonly IEntity ownerEntity;

    public ENUM_ATTACK_KEY AttackKey => skillTable.GetAttackKey(skillType);

	protected float currentCoolTime;

    public Skill(IEntity owner, ENUM_SKILL_TYPE type, CharacterSkillTable table, AddressableResourceSystem resourceSystem)
	{
		this.ownerEntity = owner;

		this.resourceSystem = resourceSystem;
        this.skillTable = table;
		this.skillType = type;

        var conditionInfo = skillTable.GetSkillConditionInfo(type);
        if (conditionInfo != null)
			maxCoolTime = conditionInfo.cooltime;

		currentCoolTime = 0.0f;

		skillTagAction = skillTable.GetSkillTagAction(type);
		characterState = skillTable.GetCharacterState(type);
	}

    private bool IsCoolTime()
    {
        return 0.0f < currentCoolTime;
    }

    public virtual bool IsSatisfied()
    {
        return IsCoolTime() == false;
    }

    public virtual void OnUpdate(float deltaTime)
    {
        currentCoolTime -= deltaTime;
		currentCoolTime = Mathf.Max(currentCoolTime, 0.0f);
	}

    public virtual void Trigger()
    {
        currentCoolTime = maxCoolTime;
        skillTagAction?.Trigger(ownerEntity);
    }
}