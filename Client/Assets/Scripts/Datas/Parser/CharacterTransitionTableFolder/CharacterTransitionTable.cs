using System;
using System.Collections.Generic;


/// <summary>
/// !주의! 수동으로 조작하지 마시오. .Helper.cs에 편의성 함수를 추가하시오.
/// </summary>
[Serializable]
[ScriptParserAttribute("CharacterTransitionTable.asset")]
public partial class CharacterTransitionTable : ScriptParser
{
	public override void Parser()
	{
		transitionDictionary.Clear();
		foreach(var value in transitionList)
		{
			transitionDictionary.Add(value.key, value);
		}
	}

	[Serializable]
	public class Transition
	{
		public int key;
		public ENUM_CHARACTER_STATE prevState;
		public ENUM_CHARACTER_STATE nextState;
		public string conditionType;
	}

	public List<Transition> transitionList = new List<Transition>();
	[System.Serializable]
	public class TransitionDictionary : SerializableDictionary<int, Transition> {}
	public TransitionDictionary transitionDictionary = new TransitionDictionary();


}
