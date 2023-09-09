using System;
using System.Collections.Generic;


/// <summary>
/// !주의! 수동으로 조작하지 마시오. .Helper.cs에 편의성 함수를 추가하시오.
/// </summary>
[Serializable]
[ScriptParserAttribute("ConditionTable.asset")]
public partial class ConditionTable : ScriptParser
{
	public override void Parser()
	{
		rawConditionDictionary.Clear();
		foreach(var value in rawConditionList)
		{
			rawConditionDictionary.Add(value.key, value);
		}
	}

	[Serializable]
	public class RawCondition
	{
		public string key;
		public float animationCondition;
		public string inputCondition;
	}

	public List<RawCondition> rawConditionList = new List<RawCondition>();
	[System.Serializable]
	public class RawConditionDictionary : SerializableDictionary<string, RawCondition> {}
	public RawConditionDictionary rawConditionDictionary = new RawConditionDictionary();


}
