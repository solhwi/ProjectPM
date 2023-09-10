using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ConditionTable : ScriptParser
{
	private Dictionary<StateConditionType, IStateCondition> compositeConditionDictionary = new Dictionary<StateConditionType, IStateCondition>();

	public override void RuntimeParser()
	{
		MakeConditions();
	}

	public void MakeConditions()
	{
		foreach (var rawCondition in rawConditionList)
		{
			if (FMUtil.TryParse(rawCondition.key, out StateConditionType conditionType) == false)
				continue;

			var condition = MakeCondition(rawCondition.key);
			if (condition != null)
			{
				condition.Parse(rawCondition);
				compositeConditionDictionary.Add(conditionType, condition);
			}
		}
	}

	private IStateCondition MakeCondition(string conditionName)
	{
		var type = Type.GetType(conditionName);

		if (type == null)
		{
			Debug.LogError($"존재하지 않는 컨디션 타입 : {conditionName}");
			return null;
		}

		if (type.IsSubclassOf(typeof(IStateCondition)) == false)
			return null;

		return (IStateCondition)Activator.CreateInstance(type);
	}
}
