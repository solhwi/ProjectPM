using StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ConditionTable;

public class CompositeStateCondition : IStateCondition
{
	private List<IStateCondition> conditions = new List<IStateCondition>();

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

	public bool Parse(string compositeRawCondition)
	{
		conditions = ConditionHelper.ParseStateConditions(compositeRawCondition).ToList();
		return true;
	}
}
