using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class SkillHelper
{
    
}

public class SkillSystem : MonoSystem
{
	[SerializeField] private AddressableResourceSystem resourceSystem = null;
    [SerializeField] private CharacterSkillTable skillTable = null;

    private Dictionary<int, List<Skill>> entitySkillDictionary = new Dictionary<int, List<Skill>>();

    protected override void OnReset()
    {
        base.OnReset();

        resourceSystem = SystemHelper.GetSystemAsset<AddressableResourceSystem>();
        skillTable = AssetDatabase.LoadAssetAtPath<CharacterSkillTable>("Assets/Bundle/Datas/Parser/CharacterSkillTable.asset");
    }

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
			var skill = MakeSkill(skillType);
			if( skill == null) 
				continue;

            skill.SetOwner(entity);
            hasSkills.Add(skill);
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
        return new Skill(skillType, skillTable, resourceSystem);
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
