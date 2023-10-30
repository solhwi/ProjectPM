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
		skillTagActionInfoDictionary.Clear();
		foreach(var value in skillTagActionInfoList)
		{
			skillTagActionInfoDictionary.Add(value.key, value);
		}
		skillEntityMappingInfoDictionary.Clear();
		foreach(var value in skillEntityMappingInfoList)
		{
			skillEntityMappingInfoDictionary.Add(value.key, value);
		}
		skillKeyMappingInfoDictionary.Clear();
		foreach(var value in skillKeyMappingInfoList)
		{
			skillKeyMappingInfoDictionary.Add(value.key, value);
		}
		skillTagActionMappingInfoDictionary.Clear();
		foreach(var value in skillTagActionMappingInfoList)
		{
			skillTagActionMappingInfoDictionary.Add(value.key, value);
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
	public class SkillTagActionInfo
	{
		public string key;
		public string parameter;
	}

	public List<SkillTagActionInfo> skillTagActionInfoList = new List<SkillTagActionInfo>();
	[System.Serializable]
	public class SkillTagActionInfoDictionary : SerializableDictionary<string, SkillTagActionInfo> {}
	public SkillTagActionInfoDictionary skillTagActionInfoDictionary = new SkillTagActionInfoDictionary();

	[Serializable]
	public class SkillEntityMappingInfo
	{
		public ENUM_SKILL_TYPE key;
		public ENUM_ENTITY_TYPE entityType;
	}

	public List<SkillEntityMappingInfo> skillEntityMappingInfoList = new List<SkillEntityMappingInfo>();
	[System.Serializable]
	public class SkillEntityMappingInfoDictionary : SerializableDictionary<ENUM_SKILL_TYPE, SkillEntityMappingInfo> {}
	public SkillEntityMappingInfoDictionary skillEntityMappingInfoDictionary = new SkillEntityMappingInfoDictionary();

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
	public class SkillTagActionMappingInfo
	{
		public ENUM_SKILL_TYPE key;
		public string skillTag;
	}

	public List<SkillTagActionMappingInfo> skillTagActionMappingInfoList = new List<SkillTagActionMappingInfo>();
	[System.Serializable]
	public class SkillTagActionMappingInfoDictionary : SerializableDictionary<ENUM_SKILL_TYPE, SkillTagActionMappingInfo> {}
	public SkillTagActionMappingInfoDictionary skillTagActionMappingInfoDictionary = new SkillTagActionMappingInfoDictionary();


}
