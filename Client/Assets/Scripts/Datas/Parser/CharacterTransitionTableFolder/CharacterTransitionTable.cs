﻿using System;
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
		priorityTransitionDictionary.Clear();
		foreach(var value in priorityTransitionList)
		{
			priorityTransitionDictionary.Add(value.key, value);
		}
		defaultTransitionDictionary.Clear();
		foreach(var value in defaultTransitionList)
		{
			defaultTransitionDictionary.Add(value.key, value);
		}
		loopTransitionDictionary.Clear();
		foreach(var value in loopTransitionList)
		{
			loopTransitionDictionary.Add(value.key, value);
		}
	}

	[Serializable]
	public class Transition
	{
		public int key;
		public CharacterState prevState;
		public CharacterState nextState;
		public string conditionType;
	}

	public List<Transition> transitionList = new List<Transition>();
	[System.Serializable]
	public class TransitionDictionary : SerializableDictionary<int, Transition> {}
	public TransitionDictionary transitionDictionary = new TransitionDictionary();

	[Serializable]
	public class PriorityTransition
	{
		public string key;
		public CharacterState nextState;
	}

	public List<PriorityTransition> priorityTransitionList = new List<PriorityTransition>();
	[System.Serializable]
	public class PriorityTransitionDictionary : SerializableDictionary<string, PriorityTransition> {}
	public PriorityTransitionDictionary priorityTransitionDictionary = new PriorityTransitionDictionary();

	[Serializable]
	public class DefaultTransition
	{
		public string key;
		public CharacterState nextState;
	}

	public List<DefaultTransition> defaultTransitionList = new List<DefaultTransition>();
	[System.Serializable]
	public class DefaultTransitionDictionary : SerializableDictionary<string, DefaultTransition> {}
	public DefaultTransitionDictionary defaultTransitionDictionary = new DefaultTransitionDictionary();

	[Serializable]
	public class LoopTransition
	{
		public CharacterState key;
		public string conditionType;
	}

	public List<LoopTransition> loopTransitionList = new List<LoopTransition>();
	[System.Serializable]
	public class LoopTransitionDictionary : SerializableDictionary<CharacterState, LoopTransition> {}
	public LoopTransitionDictionary loopTransitionDictionary = new LoopTransitionDictionary();


}
