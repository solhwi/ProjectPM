using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CharacterSkillTable : ScriptParser
{
    public IEnumerable<ENUM_SKILL_TYPE> GetSkillTypes(ENUM_ENTITY_TYPE entityType)
    {
        foreach(var mapInfo in characterSkillMapInfoList)
        {
            if (mapInfo.entityType == entityType)
            {
                yield return mapInfo.key;
            }  
        }
    }

    public bool IsUseMana(ENUM_SKILL_TYPE skillType)
    {
        return GetUseMana(skillType) > 0;
    }

    public int GetUseMana(ENUM_SKILL_TYPE skillType)
    {
        foreach (var skill in skillInfoList)
        {
            if (skillType != skill.key)
                continue;

            return skill.useMana;
        }

        return 0;
    }

    public IEnumerable<SkillInfo> GetSkills(ENUM_ENTITY_TYPE entityType)
    {
        var skillTypes = GetSkillTypes(entityType);
        if (skillTypes == null)
            yield break;

        foreach (var skillType in skillTypes)
        {
            if (skillInfoDictionary.TryGetValue(skillType, out var skillInfo))
            {
                yield return skillInfo;
            }
        }
    }
}