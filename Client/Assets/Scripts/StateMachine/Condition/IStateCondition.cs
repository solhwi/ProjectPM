using StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConditionTable;

public interface IStateCondition
{
	public bool Parse(string rawCondition);
	public bool IsSatisfied(IStateInfo stateInfo);
}

public class AnimationCondition : IStateCondition
{
	private float changeableTime = 0.0f;

	public bool IsSatisfied(IStateInfo stateInfo)
	{
		if (stateInfo is AnimationStateInfo<FrameSyncStateParam> animStateInfo)
		{
			return animStateInfo.normalizedTime >= changeableTime;
		}

		return false;
	}

	public bool Parse(string rawCondition)
	{
		return true;
	}
}

public class DashCondition : MoveCondition
{
	public override bool IsSatisfied(IStateInfo stateInfo)
	{
		bool isSatisfied = base.IsSatisfied(stateInfo);

		if (stateInfo is AnimationStateInfo<FrameSyncStateParam> animStateInfo)
		{
			isSatisfied |= animStateInfo.stateParam.userInput.isDash;
		}

		return isSatisfied;
	}
}

public class GuardCondition : IStateCondition
{
	public bool IsSatisfied(IStateInfo stateInfo)
	{
		bool isSatisfied = false;

		if (stateInfo is AnimationStateInfo<FrameSyncStateParam> animStateInfo)
		{
			isSatisfied |= animStateInfo.stateParam.userInput.isGuard;
		}

		return isSatisfied;
	}

	public bool Parse(string rawCondition)
	{
		return true;
	}
}

public class JumpUpCondition : JumpCondition
{
	public override bool IsSatisfied(IStateInfo stateInfo)
	{
		bool isSatisfied = base.IsSatisfied(stateInfo);

		if (stateInfo is AnimationStateInfo<FrameSyncStateParam> animStateInfo)
		{
			isSatisfied |= animStateInfo.stateParam.Velocity.y > Mathf.Epsilon;
		}

		return isSatisfied;
	}
}

public class JumpDownCondition : JumpCondition
{
	public override bool IsSatisfied(IStateInfo stateInfo)
	{
		bool isSatisfied = base.IsSatisfied(stateInfo);

		if (stateInfo is AnimationStateInfo<FrameSyncStateParam> animStateInfo)
		{
			isSatisfied |= animStateInfo.stateParam.Velocity.y < -1 * Mathf.Epsilon;
		}

		return isSatisfied;
	}
}

public class MoveCondition : IStateCondition
{
	private float moveVelocity;

	public virtual bool IsSatisfied(IStateInfo stateInfo)
	{
		bool isSatisfied = false;

		if (stateInfo is AnimationStateInfo<FrameSyncStateParam> animStateInfo)
		{
			float currX = animStateInfo.stateParam.Velocity.x;
			isSatisfied |= Mathf.Abs(currX) > Mathf.Abs(moveVelocity);
			isSatisfied |= animStateInfo.stateParam.IsGrounded;
		}

		return isSatisfied;
	}

	public bool Parse(string rawCondition)
	{
		return true;
	}
}

public class JumpCondition : IStateCondition
{
	public virtual bool IsSatisfied(IStateInfo stateInfo)
	{
		if (stateInfo is AnimationStateInfo<FrameSyncStateParam> animStateInfo)
		{
			return animStateInfo.stateParam.IsGrounded == false;
		}

		return false;
	}

	public bool Parse(string rawCondition)
	{
		return true;
	}
}

public class AttackCondition : IStateCondition
{
	private ENUM_ATTACK_KEY key = ENUM_ATTACK_KEY.NONE;

	public bool IsSatisfied(IStateInfo stateInfo)
	{
		if(stateInfo is AnimationStateInfo<FrameSyncStateParam> animStateInfo)
		{
			return animStateInfo.stateParam.userInput.pressedAttackKey == key;
		}

		return false;
	}

	public bool Parse(string rawCondition)
	{
		return true;
	}
}