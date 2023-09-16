using NPOI.SS.Formula.PTG;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class ConditionTable : ScriptParser
{
	private Dictionary<string, IStateCondition> conditionDictionary = new Dictionary<string, IStateCondition>();
	private const char ParameterSeparator = ':';
	private const char ConditionSeparator = '/';

	public IStateCondition GetCondition(string rawConditionType)
	{
		if (conditionDictionary.TryGetValue(rawConditionType, out var condition))
		{
			return condition;
		}

		Debug.LogError($"존재하지 않는 컨디션 타입 : {rawConditionType}");
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
			var conditionType = rawCondition.key.Trim();
			var condition = MakeStateCondition(conditionType, out string[] parameters);
			if (condition != null)
			{
				condition.Parse(rawCondition.condition);
				conditionDictionary.Add(conditionType, condition);
			}
		}
	}

	public IEnumerable<IStateCondition> ParseStateConditions(string compositeRawCondition)
	{
		foreach (var rawCondition in compositeRawCondition.Split(ConditionSeparator))
		{
			var stateCondition = GetCondition(rawCondition);
			if (stateCondition == null)
			{
				stateCondition = MakeStateCondition(rawCondition, out string[] parameters);
				stateCondition.Parse(parameters);
			}

			yield return stateCondition;
		}
	}

	private IStateCondition MakeStateCondition(string rawCondition, out string[] parameters)
	{
		string[] rawConditions = rawCondition.Split(ParameterSeparator);

		string inputRawConditionType = rawConditions[0];
		parameters = rawConditions.Where(raw => raw.Equals(inputRawConditionType) == false).ToArray();

		switch (inputRawConditionType)
		{
			case "[AnimationWait]":
				return new AnimationWaitCondition();

			case "[Attack]":
				return new AttackCondition();

			case "[JumpUp]":
				return new JumpUpCondition();

			case "[JumpDown]":
				return new JumpDownCondition();

			case "[Guard]":
				return new GuardCondition();

			case "[Move]":
				return new MoveCondition();

			case "[Dash]":
				return new DashCondition();

			case "[Combo]":
				return new CompositeStateCondition(this);

			default:
				return null;
		}
	}
}
