using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CharacterSkillTable : ScriptParser
{
    private readonly Dictionary<string, ISkillTagAction> skillTagActionDictionary = new Dictionary<string, ISkillTagAction>();
    
    private const char ParameterSeparator = ':';
    private const char ParameterOrSeparator = '+';

    public override void RuntimeParser()
    {
        MakeSkillTagInfos();
    }

    public void MakeSkillTagInfos()
    {
        foreach (var skillTagInfo in skillTagActionInfoList)
        {
            var skillTagType = skillTagInfo.key.Trim();
            var skillTagAction = MakeSkillTagAction(skillTagType, out string[] parameters);
            if (skillTagAction != null)
            {
                skillTagAction.Parse(skillTagInfo.parameter);
                skillTagActionDictionary.Add(skillTagType, skillTagAction);
            }
        }
    }

    private ISkillTagAction MakeSkillTagAction(string skillTagType, out string[] parameters)
    {
        string[] rawSkillTags = skillTagType.Split(ParameterSeparator);

        string rawSkillTagType = rawSkillTags[0];
        parameters = rawSkillTags.Where(raw => raw.Equals(rawSkillTagType) == false).ToArray();

        switch (rawSkillTagType)
        {
            case "[Instantiate]":
                return new InstantiateTagAction();

            case "[Pencil Instantiate]":
                return new CompositeSkillTagAction(this);
        }

        Debug.LogError($"정의되지 않은 스킬 액션 타입 : {skillTagType}");
        return null;
    }

    public ENUM_CHARACTER_STATE GetCharacterState(ENUM_SKILL_TYPE skillType)
    {
		if (skillConditionInfoDictionary.TryGetValue(skillType, out var conditionInfo))
		{
			return conditionInfo.characterState;
		}

        return ENUM_CHARACTER_STATE.None;
	}

    public ENUM_ATTACK_KEY GetAttackKey(ENUM_SKILL_TYPE skillType)
    {
        if (skillKeyMappingInfoDictionary.TryGetValue(skillType, out var mappingInfo))
        {
            return mappingInfo.attackKeyType;
        }

        return ENUM_ATTACK_KEY.NONE;
    }
    public IEnumerable<ENUM_SKILL_TYPE> GetSkillTypes(ENUM_ENTITY_TYPE entityType)
    {
        return skillEntityMappingInfoList.Where(m => m.entityType == entityType).Select(m => m.key);
	}

	public ENUM_SKILL_TYPE GetSkillType(ENUM_ENTITY_TYPE entityType, ENUM_ATTACK_KEY keyType)
	{
        var skillTypes = GetSkillTypes(entityType);
        if (skillTypes == null)
            return ENUM_SKILL_TYPE.None;

        foreach(var skillType in skillTypes)
        {
			foreach(var skillKeyMappingInfo in skillKeyMappingInfoList)
            {
                if (skillKeyMappingInfo.attackKeyType != keyType || skillKeyMappingInfo.key != skillType)
                    continue;

				return skillType;
			}
		}

        return ENUM_SKILL_TYPE.None;
	}

	public SkillConditionInfo GetSkillConditionInfo(ENUM_SKILL_TYPE skillType)
    {
        if(skillConditionInfoDictionary.ContainsKey(skillType))
        {
            return skillConditionInfoDictionary[skillType];
        }

        Debug.LogError($"존재하지 않는 스킬 컨디션 타입 : {skillType}");
        return null;
    }

    public SkillTagActionMappingInfo GetSkillActionInfo(ENUM_SKILL_TYPE skillType)
    {
        if (skillTagActionMappingInfoDictionary.ContainsKey(skillType)) 
        {
            return skillTagActionMappingInfoDictionary[skillType];
        }

        return null;
    }

    public bool IsUseManaSkill(ENUM_SKILL_TYPE skillType)
    {
        return GetUseMana(skillType) > 0;
    }

    public int GetUseMana(ENUM_SKILL_TYPE skillType)
    {
        foreach (var skill in skillConditionInfoList)
        {
            if (skillType != skill.key)
                continue;

            return skill.useMana;
        }

        return 0;
    }

    public IEnumerable<SkillConditionInfo> GetSkillConditionInfos(ENUM_ENTITY_TYPE entityType)
    {
        var skillTypes = GetSkillTypes(entityType);
        if (skillTypes == null)
            yield break;

        foreach (var skillType in skillTypes)
        {
            if (skillConditionInfoDictionary.TryGetValue(skillType, out var skillInfo))
            {
                yield return skillInfo;
            }
        }
    }

    public SkillConditionInfo GetSkillConditionInfo(ENUM_ENTITY_TYPE entityType, ENUM_ATTACK_KEY attackKey)
    {
        var skillType = GetSkillType(entityType, attackKey);
        if (skillType == ENUM_SKILL_TYPE.None)
            return null;

        if (skillConditionInfoDictionary.TryGetValue(skillType, out var skillInfo) == false)
            return null;

		return skillInfo;
	}

	public ISkillTagAction GetSkillTagAction(string skillTag)
    {
		if (skillTagActionDictionary.TryGetValue(skillTag, out var skillTagAction) == false)
			return null;

        return skillTagAction;
	}

    public ISkillTagAction GetSkillTagAction(ENUM_SKILL_TYPE skillType)
    {
        var skillActionInfo = GetSkillActionInfo(skillType);
        if (skillActionInfo == null)
            return null;

        return GetSkillTagAction(skillActionInfo.skillTag);
    }

    public IEnumerable<ISkillTagAction> ParseSkillTagActions(string compositeRawSkillTagAction)
    {
        foreach (var orSeparatedSkillTagAction in compositeRawSkillTagAction.Split(ParameterOrSeparator))
        {
            var skillTagAction = MakeSkillTagAction(orSeparatedSkillTagAction, out string[] parameters);
            skillTagAction.Parse(parameters);

            yield return skillTagAction;
        }
    }
}
