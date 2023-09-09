using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ConditionTable : ScriptParser
{
	[SerializeField] private List<ICompositeCondition> compositeConditions = new List<ICompositeCondition>();

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

	private ICompositeCondition MakeCondition(string conditionName)
	{
		var type = Type.GetType(conditionName);

		if (type == null)
			return null;

		if (type.IsSubclassOf(typeof(ICompositeCondition)) == false)
			return null;

		return (ICompositeCondition)Activator.CreateInstance(type);
	}
}
