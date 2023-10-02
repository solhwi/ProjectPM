using System;
using System.Collections.Generic;


/// <summary>
/// !주의! 수동으로 조작하지 마시오. .Helper.cs에 편의성 함수를 추가하시오.
/// </summary>
[Serializable]
[ScriptParserAttribute("EntityConditionTable.asset")]
public partial class EntityConditionTable : ScriptParser
{
	public override void Parser()
	{
		characterRawconditionDictionary.Clear();
		foreach(var value in characterRawconditionList)
		{
			characterRawconditionDictionary.Add(value.key, value);
		}
	}

	[Serializable]
	public class CharacterRawCondition
	{
		public string key;
		public string condition;
	}

	public List<CharacterRawCondition> characterRawconditionList = new List<CharacterRawCondition>();
	[System.Serializable]
	public class CharacterRawConditionDictionary : SerializableDictionary<string, CharacterRawCondition> {}
	public CharacterRawConditionDictionary characterRawconditionDictionary = new CharacterRawConditionDictionary();


}
