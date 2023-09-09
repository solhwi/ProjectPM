using StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateCondition
{
	public bool IsSatisfied(IStateInfo input);
}

public class AnimationCondition : IStateCondition
{
	private float changeableTime = 0.0f;

	public AnimationCondition(float changeableTime)
	{
		this.changeableTime = changeableTime;
	}

	public bool IsSatisfied(IStateInfo stateInfo)
	{
		if (stateInfo is AnimationStateInfo<FrameSyncStateParam> animStateInfo)
		{
			return animStateInfo.normalizedTime >= changeableTime;
		}

		return false;
	}
}

public class AttackCondition : IStateCondition
{
	private ENUM_ATTACK_KEY key;

	public AttackCondition(ENUM_ATTACK_KEY key)
	{
		this.key = key;
	}

	public bool IsSatisfied(IStateInfo stateInfo)
	{
		if(stateInfo is AnimationStateInfo<FrameSyncStateParam> animStateInfo)
		{
			return animStateInfo.stateParam.userInput.pressedAttackKey == key;
		}

		return false;
	}
}