using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using UnityEngine;
using static ConditionTable;

public enum StateConditionType
{
	None = -1,

	// 디폴트 타입
	AnimationWait,
	Attack,
	Jump,
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
			if (type == StateConditionType.None)
				continue;

			var stateCondition = MakeStateCondition(type);
			if (stateCondition == null)
				continue;

			stateCondition.Parse(rawCondition);

			yield return stateCondition;
		}
	}

	public static StateConditionType ConvertStateConditionType(string rawCondition)
	{
		string[] rawConditions = rawCondition.Split(ParameterSeparator);

		string inputRawConditionType = rawConditions[0];
		switch (inputRawConditionType)
		{
			case "[AnimationWait]":
				return StateConditionType.AnimationWait;

			case "[Attack]":
				return StateConditionType.Attack;

			case "[Jump]":
				return StateConditionType.Jump;

			case "[JumpUp]":
				return StateConditionType.JumpUp;

			case "[JumpDown]":
				return StateConditionType.JumpDown;

			case "[Guard]":
				return StateConditionType.Guard;

			case "[Move]":
				return StateConditionType.Move;

			case "[Dash]":
				return StateConditionType.Dash;

			case "[Combo]":
				return StateConditionType.Combo;

			case "[AnimationAllWait]":
				return StateConditionType.AnimationAllWait;

			default:
				return StateConditionType.None;
		}
	}

	public static IStateCondition MakeStateCondition(StateConditionType stateType)
	{
		switch (stateType)
		{
			case StateConditionType.Combo:
			case StateConditionType.AnimationAllWait:
				return new CompositeStateCondition();

			case StateConditionType.AnimationWait:
				return new AnimationCondition();

			case StateConditionType.Attack:
				return new AttackCondition();

			case StateConditionType.Jump:
				return new JumpCondition();

			case StateConditionType.JumpUp:
				return new JumpUpCondition();

			case StateConditionType.JumpDown:
				return new JumpDownCondition();

			case StateConditionType.Guard:
				return new GuardCondition();

			case StateConditionType.Move:
				return new MoveCondition();

			case StateConditionType.Dash:
				return new DashCondition();

			case StateConditionType.None:
			default:
				return null;
		}
	}
}


public partial class ConditionTable : ScriptParser
{
	private Dictionary<StateConditionType, IStateCondition> conditionDictionary = new Dictionary<StateConditionType, IStateCondition>();

	public override void RuntimeParser()
	{
		MakeConditions();
	}

	public void MakeConditions()
	{
		foreach (var rawCondition in rawConditionList)
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
