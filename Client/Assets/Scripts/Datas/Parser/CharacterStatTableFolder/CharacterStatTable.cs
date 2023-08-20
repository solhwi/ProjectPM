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
		CharacterStatDictionary.Clear();
		foreach(var value in CharacterStatList)
		{
			CharacterStatDictionary.Add(value.key, value);
		}
	}

	[Serializable]
	public class CharacterStat
	{
		public int key;
		public int moveSpeed;
		public int attackSpeed;
		public int jumpPower;
		public int damage;
	}

	public List<CharacterStat> CharacterStatList = new List<CharacterStat>();
	[System.Serializable]
	public class SCharacterStatDictionary : SerializableDictionary<int, CharacterStat> {}
	public SCharacterStatDictionary CharacterStatDictionary = new SCharacterStatDictionary();


}
