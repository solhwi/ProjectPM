using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CharacterSkillTable : ScriptParser
{
    public IEnumerable<ENUM_SKILL_TYPE> GetSkillTypes(ENUM_ENTITY_TYPE entityType)
    {
        return entitySkillMappingInfoList.Where(m => m.entityType == entityType).Select(m => m.key);
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

        Debug.LogError($"존재하지 않는 스킬 타입 : {skillType}");
        return null;
    }

    public bool IsUseMana(ENUM_SKILL_TYPE skillType)
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
}
