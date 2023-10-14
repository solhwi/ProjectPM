using System;
using System.Collections.Generic;


/// <summary>
/// !주의! 수동으로 조작하지 마시오. .Helper.cs에 편의성 함수를 추가하시오.
/// </summary>
[Serializable]
[ScriptParserAttribute("CharacterStatTable.asset")]
public partial class CharacterStatTable : ScriptParser
{
	public override void Parser()
	{
		characterStatDictionary.Clear();
		foreach(var value in characterStatList)
		{
			characterStatDictionary.Add(value.key, value);
		}
	}

	[Serializable]
	public class CharacterStat
	{
		public ENUM_ENTITY_TYPE key;
		public int jumpPower;
		public int mass;
		public int moveSpeed;
		public int attackSpeed;
	}

	public List<CharacterStat> characterStatList = new List<CharacterStat>();
	[System.Serializable]
	public class CharacterStatDictionary : SerializableDictionary<ENUM_ENTITY_TYPE, CharacterStat> {}
	public CharacterStatDictionary characterStatDictionary = new CharacterStatDictionary();


}
