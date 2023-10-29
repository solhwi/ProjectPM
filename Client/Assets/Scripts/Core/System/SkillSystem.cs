using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillHelper
{
    
}

public class SkillSystem : MonoSystem
{
    [SerializeField] private CharacterSkillTable skillTable = null;

    private Dictionary<int, List<Skill>> entitySkillDictionary = new Dictionary<int, List<Skill>>();

    public void Register(IEntity entity)
    {
        if (entity == null)
            return;

        var skillTypes = skillTable.GetSkillTypes(entity.EntityType);
        if (skillTypes == null)
            return;

		List<Skill> hasSkills = new();

		foreach (var skillType in skillTypes)
		{
			hasSkills.Add(MakeSkill(skillType));
		}

		entitySkillDictionary[entity.EntityGuid] = hasSkills;
	}

    public void Unregister(IEntity entity)
    {
        if(entity == null) 
            return;

        if(entitySkillDictionary.ContainsKey(entity.EntityGuid))
            entitySkillDictionary.Remove(entity.EntityGuid);
    }

	public Skill MakeSkill(ENUM_SKILL_TYPE skillType)
	{
		switch (skillType)
		{
			case ENUM_SKILL_TYPE.SlashPencil:
				return new SlashPencil(skillType, skillTable);

			case ENUM_SKILL_TYPE.ThrowPencil:
				return new ThrowPencil(skillType, skillTable);
		}

		Debug.LogError($"정의되지 않은 스킬 타입 : {skillType}");
		return null;
	}

	public override void OnUpdate(int deltaFrameCount, float deltaTime)
	{
		base.OnUpdate(deltaFrameCount, deltaTime);

		foreach (var skills in entitySkillDictionary.Values)
		{
			foreach (var skill in skills)
			{
				skill.OnUpdate(deltaTime);
			}
		}
	}
}
