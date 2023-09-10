using StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConditionTable;

public enum StateConditionType
{
	ComboCondition,
	WaitCondition,
}

public class CompositeStateCondition : IStateCondition
{
	private List<IStateCondition> conditions = new List<IStateCondition>();
	private const char parameterSeparator = ':';
	private const char conditionSeparator = '/';

	public bool IsSatisfied(IStateInfo stateInfo)
	{
		foreach (var condition in conditions)
		{
			if (condition.IsSatisfied(stateInfo) == false)
			{
				return false;
			}
		}

		return true;
	}

	public bool Parse(RawCondition rawCondition)
	{
		conditions.Clear();

		foreach (var stateCondition in ParseStateConditions(rawCondition.condition))
		{
			conditions.Add(stateCondition);
		}

		return true;
	}

	private IEnumerable<IStateCondition> ParseStateConditions(string rawConditions)
	{
		foreach (var rawCondition in rawConditions.Split(conditionSeparator))
		{
			yield return MakeInputCondition(rawCondition);
		}
	}

	private IStateCondition MakeInputCondition(string inputRawCondition)
	{
		string[] inputRawConditions = inputRawCondition.Split(parameterSeparator);

		string inputRawConditionType = inputRawConditions[0];
		switch (inputRawConditionType)
		{
			case "[Animation]":

				float normalizeTime = TryGetFloat(inputRawConditions);
				return new AnimationCondition(normalizeTime);

			case "[Attack]":
				return new NormalAttackCondition();

			case "[Skill]":
				return new SkillAttackCondition();

			case "[Ultimate]":
				return new UltimateAttackCondition();

			case "[Jump]":
				return new JumpCondition();

			case "[JumpUp]":
				return new JumpUpCondition();

			case "[JumpDown]":
				return new JumpDownCondition();

			case "[Guard]":
				return new GuardCondition();

			case "[Move]":

				float moveVelocity = TryGetFloat(inputRawConditions);
				return new MoveCondition(moveVelocity);

			case "[Dash]":

				float dashVelocity = TryGetFloat(inputRawConditions);
				return new DashCondition(dashVelocity);

			default: // None을 여기서 처리한다.
				return null;
		}
	}

	private float TryGetFloat(string[] parameters)
	{
		float value = Mathf.Epsilon;

		if (parameters.Length > 1)
		{
			value = TryParseParameter<float>(parameters[1]);
		}

		return value;
	}

	private T TryParseParameter<T>(string parameter)
	{ 		
		if(typeof(T) == typeof(float))
		{
			if (float.TryParse(parameter, out float floatValue) == false)
			{
				Debug.LogError($"컨디션의 파라미터를 파싱하는 과정에서 문제가 발생했습니다. 결과 : {parameter} -> {floatValue}");
			}

			return (T)(object)floatValue;
		}

		return default;
	}

}
