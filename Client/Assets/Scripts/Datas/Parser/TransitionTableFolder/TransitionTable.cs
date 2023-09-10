using System;
using System.Collections.Generic;


/// <summary>
/// !주의! 수동으로 조작하지 마시오. .Helper.cs에 편의성 함수를 추가하시오.
/// </summary>
[Serializable]
[ScriptParserAttribute("TransitionTable.asset")]
public partial class TransitionTable : ScriptParser
{
	public override void Parser()
	{
		characterTransitionDictionary.Clear();
		foreach(var value in characterTransitionList)
		{
			characterTransitionDictionary.Add(value.key, value);
		}
	}

	[Serializable]
	public class CharacterTransition
	{
		public int key;
		public CharacterState prevState;
		public CharacterState nextState;
		public string conditionType;
	}

	public List<CharacterTransition> characterTransitionList = new List<CharacterTransition>();
	[System.Serializable]
	public class CharacterTransitionDictionary : SerializableDictionary<int, CharacterTransition> {}
	public CharacterTransitionDictionary characterTransitionDictionary = new CharacterTransitionDictionary();


}
