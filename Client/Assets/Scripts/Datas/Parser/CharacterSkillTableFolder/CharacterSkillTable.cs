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
		skillInfoDictionary.Clear();
		foreach(var value in skillInfoList)
		{
			skillInfoDictionary.Add(value.key, value);
		}
		characterSkillMapInfoDictionary.Clear();
		foreach(var value in characterSkillMapInfoList)
		{
			characterSkillMapInfoDictionary.Add(value.key, value);
		}
	}

	[Serializable]
	public class SkillInfo
	{
		public ENUM_SKILL_TYPE key;
		public float searchBoxX;
		public float searchBoxY;
		public float searchOffsetX;
		public float searchOffsetY;
		public int useMana;
	}

	public List<SkillInfo> skillInfoList = new List<SkillInfo>();
	[System.Serializable]
	public class SkillInfoDictionary : SerializableDictionary<ENUM_SKILL_TYPE, SkillInfo> {}
	public SkillInfoDictionary skillInfoDictionary = new SkillInfoDictionary();

	[Serializable]
	public class CharacterSkillMapInfo
	{
		public ENUM_SKILL_TYPE key;
		public ENUM_ENTITY_TYPE entityType;
	}

	public List<CharacterSkillMapInfo> characterSkillMapInfoList = new List<CharacterSkillMapInfo>();
	[System.Serializable]
	public class CharacterSkillMapInfoDictionary : SerializableDictionary<ENUM_SKILL_TYPE, CharacterSkillMapInfo> {}
	public CharacterSkillMapInfoDictionary characterSkillMapInfoDictionary = new CharacterSkillMapInfoDictionary();


}
