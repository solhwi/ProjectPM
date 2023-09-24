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

	// 여기서 & 혹은 | 로 만족 체크를 하도록 해야 한다.
	public bool IsSatisfied(IStateInfo stateInfo)
	{
		bool isSatisfied = true;

		foreach (var condition in conditions)
		{
			isSatisfied |= condition.IsSatisfied(stateInfo);
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
