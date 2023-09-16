using StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CompositeStateCondition : IStateCondition
{
	private ConditionTable conditionTable;
	private List<IStateCondition> conditions = new List<IStateCondition>();

	public CompositeStateCondition(ConditionTable table) 
	{
		conditionTable = table;
	}

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

	public bool Parse(params string[] compositeRawCondition)
	{
		if(compositeRawCondition.Any() == false)
			return false;

		conditions = conditionTable.ParseStateConditions(compositeRawCondition[0]).ToList();
		return true;
	}
}
