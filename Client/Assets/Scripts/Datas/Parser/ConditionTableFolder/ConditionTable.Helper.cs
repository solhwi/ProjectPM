using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterStateConditionType
{
	None = -1,

	// 디폴트 타입
	AnimationWait,
	Attack,
	JumpUp,
	JumpDown,
	Guard,
	Move,
	Dash,

	// 새로 정의된 타입
	Combo = 100,
	AnimationAllWait,
}

public class ConditionHelper
{
	private const char ParameterSeparator = ':';
	private const char ConditionSeparator = '/';

	public static IEnumerable<IStateCondition> ParseStateConditions(string compositeRawCondition)
	{
		foreach (var rawCondition in compositeRawCondition.Split(ConditionSeparator))
		{
			var type = ConvertStateConditionType(rawCondition);
			if (type == CharacterStateConditionType.None)
				continue;

			var stateCondition = MakeStateCondition(type);
			if (stateCondition == null)
				continue;

			stateCondition.Parse(rawCondition);

			yield return stateCondition;
		}
	}

	public static CharacterStateConditionType ConvertStateConditionType(string rawCondition)
	{
		string[] rawConditions = rawCondition.Split(ParameterSeparator);

		string inputRawConditionType = rawConditions[0];
		switch (inputRawConditionType)
		{
			case "[AnimationWait]":
				return CharacterStateConditionType.AnimationWait;

			case "[Attack]":
				return CharacterStateConditionType.Attack;

			case "[JumpUp]":
				return CharacterStateConditionType.JumpUp;

			case "[JumpDown]":
				return CharacterStateConditionType.JumpDown;

			case "[Guard]":
				return CharacterStateConditionType.Guard;

			case "[Move]":
				return CharacterStateConditionType.Move;

			case "[Dash]":
				return CharacterStateConditionType.Dash;

			case "[Combo]":
				return CharacterStateConditionType.Combo;

			case "[AnimationAllWait]":
				return CharacterStateConditionType.AnimationAllWait;

			default:
				return CharacterStateConditionType.None;
		}
	}

	public static IStateCondition MakeStateCondition(CharacterStateConditionType stateType)
	{
		switch (stateType)
		{
			case CharacterStateConditionType.Combo:
			case CharacterStateConditionType.AnimationAllWait:
				return new CompositeStateCondition();

			case CharacterStateConditionType.AnimationWait:
				return new AnimationCondition();

			case CharacterStateConditionType.Attack:
				return new AttackCondition();

			case CharacterStateConditionType.JumpUp:
				return new JumpUpCondition();

			case CharacterStateConditionType.JumpDown:
				return new JumpDownCondition();

			case CharacterStateConditionType.Guard:
				return new GuardCondition();

			case CharacterStateConditionType.Move:
				return new MoveCondition();

			case CharacterStateConditionType.Dash:
				return new DashCondition();

			case CharacterStateConditionType.None:
			default:
				return null;
		}
	}
}


public partial class ConditionTable : ScriptParser
{
	private Dictionary<CharacterStateConditionType, IStateCondition> conditionDictionary = new Dictionary<CharacterStateConditionType, IStateCondition>();

	public IStateCondition GetCondition(CharacterStateConditionType conditionType)
	{
		if(conditionDictionary.TryGetValue(conditionType, out var condition))
		{
			return condition;
		}

		Debug.LogError($"존재하지 않는 컨디션 타입 : {conditionType}");
		return null;
	}

	public IStateCondition GetCondition(string rawCondition)
	{
		var conditionType = ConditionHelper.ConvertStateConditionType(rawCondition);
		if (conditionDictionary.TryGetValue(conditionType, out var condition))
		{
			return condition;
		}

		Debug.LogError($"존재하지 않는 컨디션 타입 : {conditionType}");
		return null;
	}


	public override void RuntimeParser()
	{
		MakeConditions();
	}

	public void MakeConditions()
	{
		foreach (var rawCondition in characterRawconditionList)
		{
			var conditionType = ConditionHelper.ConvertStateConditionType(rawCondition.key.Trim());

			var condition = ConditionHelper.MakeStateCondition(conditionType);
			if (condition != null)
			{
				condition.Parse(rawCondition.condition.Trim());
				conditionDictionary.Add(conditionType, condition);
			}
		}
	}
}
