using System;
using System.Collections.Generic;


/// <summary>
/// !주의! 수동으로 조작하지 마시오. .Helper.cs에 편의성 함수를 추가하시오.
/// </summary>
[Serializable]
[ScriptParserAttribute("CharacterSkillTable.asset")]
public partial class CharacterSkillTable : ScriptParser
{
	public override void Parser()
	{
		skillConditionInfoDictionary.Clear();
		foreach(var value in skillConditionInfoList)
		{
			skillConditionInfoDictionary.Add(value.key, value);
		}
		entitySkillMappingInfoDictionary.Clear();
		foreach(var value in entitySkillMappingInfoList)
		{
			entitySkillMappingInfoDictionary.Add(value.key, value);
		}
		skillKeyMappingInfoDictionary.Clear();
		foreach(var value in skillKeyMappingInfoList)
		{
			skillKeyMappingInfoDictionary.Add(value.key, value);
		}
		skillTagInfoDictionary.Clear();
		foreach(var value in skillTagInfoList)
		{
			skillTagInfoDictionary.Add(value.key, value);
		}
		skillActionInfoDictionary.Clear();
		foreach(var value in skillActionInfoList)
		{
			skillActionInfoDictionary.Add(value.key, value);
		}
	}

	[Serializable]
	public class SkillConditionInfo
	{
		public ENUM_SKILL_TYPE key;
		public float searchBoxX;
		public float searchBoxY;
		public float searchOffsetX;
		public float searchOffsetY;
		public int useMana;
		public float cooltime;
	}

	public List<SkillConditionInfo> skillConditionInfoList = new List<SkillConditionInfo>();
	[System.Serializable]
	public class SkillConditionInfoDictionary : SerializableDictionary<ENUM_SKILL_TYPE, SkillConditionInfo> {}
	public SkillConditionInfoDictionary skillConditionInfoDictionary = new SkillConditionInfoDictionary();

	[Serializable]
	public class EntitySkillMappingInfo
	{
		public ENUM_SKILL_TYPE key;
		public ENUM_ENTITY_TYPE entityType;
	}

	public List<EntitySkillMappingInfo> entitySkillMappingInfoList = new List<EntitySkillMappingInfo>();
	[System.Serializable]
	public class EntitySkillMappingInfoDictionary : SerializableDictionary<ENUM_SKILL_TYPE, EntitySkillMappingInfo> {}
	public EntitySkillMappingInfoDictionary entitySkillMappingInfoDictionary = new EntitySkillMappingInfoDictionary();

	[Serializable]
	public class SkillKeyMappingInfo
	{
		public ENUM_SKILL_TYPE key;
		public ENUM_ATTACK_KEY attackKeyType;
	}

	public List<SkillKeyMappingInfo> skillKeyMappingInfoList = new List<SkillKeyMappingInfo>();
	[System.Serializable]
	public class SkillKeyMappingInfoDictionary : SerializableDictionary<ENUM_SKILL_TYPE, SkillKeyMappingInfo> {}
	public SkillKeyMappingInfoDictionary skillKeyMappingInfoDictionary = new SkillKeyMappingInfoDictionary();

	[Serializable]
	public class SkillTagInfo
	{
		public string key;
		public string parameter;
	}

	public List<SkillTagInfo> skillTagInfoList = new List<SkillTagInfo>();
	[System.Serializable]
	public class SkillTagInfoDictionary : SerializableDictionary<string, SkillTagInfo> {}
	public SkillTagInfoDictionary skillTagInfoDictionary = new SkillTagInfoDictionary();

	[Serializable]
	public class SkillActionInfo
	{
		public ENUM_SKILL_TYPE key;
		public string skillTag;
	}

	public List<SkillActionInfo> skillActionInfoList = new List<SkillActionInfo>();
	[System.Serializable]
	public class SkillActionInfoDictionary : SerializableDictionary<ENUM_SKILL_TYPE, SkillActionInfo> {}
	public SkillActionInfoDictionary skillActionInfoDictionary = new SkillActionInfoDictionary();


}
