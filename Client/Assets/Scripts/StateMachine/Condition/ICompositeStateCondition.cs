using StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConditionTable;

public abstract class ICompositeStateCondition
{
	protected List<IStateCondition> conditions = new List<IStateCondition>();

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

	public abstract bool Parse(RawCondition rawCondition);
}

public class ComboCondition : ICompositeStateCondition
{
	public override bool Parse(RawCondition rawCondition)
	{
		if(rawCondition.animationCondition > 0.0f)
		{
			conditions.Add(new AnimationCondition(rawCondition.animationCondition));
		}

		var inputCondition = MakeInputCondition(rawCondition.inputCondition);
		if (inputCondition != null)
		{
			conditions.Add(inputCondition);
		}	

		return true;
	}

	private IStateCondition MakeInputCondition(string inputRawCondition)
	{
		switch(inputRawCondition)
		{
			case "Attack":
				return new AttackCondition(ENUM_ATTACK_KEY.ATTACK);

			case "None":
				return null;
		}

		return null;
	}
}

