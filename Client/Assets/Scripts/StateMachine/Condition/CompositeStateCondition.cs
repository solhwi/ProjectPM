using StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CompositeStateCondition : IStateCondition
{
	private CharacterConditionTable conditionTable;
	private List<KeyValuePair<IStateCondition, bool>> conditions = new List<KeyValuePair<IStateCondition, bool>>();

	public CompositeStateCondition(CharacterConditionTable table) 
	{
		conditionTable = table;
	}

	public bool IsSatisfied(FrameCommandMessage stateInfo)
	{
		bool isSatisfied = false;

		for(int i = 0; i < conditions.Count; i++)
		{
			var conditionPair = conditions[i];

			var condition = conditionPair.Key;
			bool isAndCondition = conditionPair.Value;

			if (i == 0) // 첫 컨디션은 &도 아니고 |도 아님
			{
				isSatisfied = condition.IsSatisfied(stateInfo);
			}
			else if (isAndCondition)
			{
				isSatisfied &= condition.IsSatisfied(stateInfo);
			}
			else
			{
				isSatisfied |= condition.IsSatisfied(stateInfo);
			}
		}

		return isSatisfied;
	}

	public bool Parse(params string[] compositeRawCondition)
	{
		if(compositeRawCondition.Any() == false)
			return false;

		conditions = conditionTable.ParseStateConditions(compositeRawCondition[0]).ToList();
		return true;
	}
}
