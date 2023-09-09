using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ConditionTable : ScriptParser
{
	private List<ICompositeStateCondition> compositeConditions = new List<ICompositeStateCondition>();

	public override void RuntimeParser()
	{
		MakeConditions();
	}

	public void MakeConditions()
	{
		foreach (var rawCondition in rawConditionList)
		{
			var condition = MakeCondition(rawCondition.key);
			if (condition != null)
			{
				condition.Parse(rawCondition);
				compositeConditions.Add(condition);
			}
		}
	}

	private ICompositeStateCondition MakeCondition(string conditionName)
	{
		var type = Type.GetType(conditionName);

		if (type == null)
			return null;

		if (type.IsSubclassOf(typeof(ICompositeStateCondition)) == false)
			return null;

		return (ICompositeStateCondition)Activator.CreateInstance(type);
	}
}
