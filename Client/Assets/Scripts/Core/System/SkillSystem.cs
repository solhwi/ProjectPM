using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        resourceSystem = AssetLoadHelper.GetSystemAsset<AddressableResourceSystem>();
        skillTable = AssetLoadHelper.GetDataAsset<CharacterSkillTable>();
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
			var skill = MakeSkill(skillType, entity);
			if (skill == null) 
				continue;

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

    private IEnumerable<Skill> GetUsableSkills(IEntity entity)
    {
		if (entitySkillDictionary.TryGetValue(entity.EntityGuid, out var skillList) == false)
		{
			Debug.LogError($"{entity.OwnerGuid}는 스킬 시스템에 등록되지 않은 앤티티입니다.");
            yield break;
		}

		foreach (var skill in skillList)
		{
			if (skill.IsSatisfied())
			{
				yield return skill;
			}
		}
	}
    
    private IEnumerable<ENUM_CHARACTER_STATE> GetUsableSkillStates(IEntity entity)
    {
        return GetUsableSkills(entity).Select(skill => skill.characterState);
    }

    public bool IsUsableSkillState(IEntity entity, ENUM_CHARACTER_STATE characterState)
    {
		return GetUsableSkillStates(entity).Any(state => state == characterState);
	}    

    public bool UseSkill(IEntity owner, ENUM_SKILL_TYPE skillType)
    {
        if (entitySkillDictionary.TryGetValue(owner.EntityGuid, out var skillList) == false)
        {
            Debug.LogError($"{owner.OwnerGuid}는 스킬 시스템에 등록되지 않은 앤티티입니다.");
            return false;
        }

        var skill = skillList.FirstOrDefault(skill => skill.skillType == skillType);
        if (skill == null)
        {
            Debug.LogError($"{owner.OwnerGuid}가 가지지 않은 스킬 타입 : {skillType}");
            return false;
        }

        if (skill.IsSatisfied() == false)
        {
            Debug.LogError($"{skillType} 현재 사용할 수 없는 스킬입니다.");
            return false;
        }

        skill.Trigger();
        return true;
    }

	private Skill MakeSkill(ENUM_SKILL_TYPE skillType, IEntity entity)
	{
        return new Skill(entity, skillType, skillTable, resourceSystem);
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
