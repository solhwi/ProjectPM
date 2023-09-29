using System.Linq;
using UnityEngine;

public interface IStateCondition
{
	public bool Parse(params string[] rawParameters);
	public bool IsSatisfied(IStateInfo stateInfo);
}

public abstract class FloatParameterStateCondition : IStateCondition
{
    protected float value = 0.0f;

	public abstract bool IsSatisfied(IStateInfo stateInfo);

    public bool Parse(params string[] rawParameters)
    {
        if (float.TryParse(rawParameters[0], out value))
        {
            return true;
        }

        return false;
    }
}

public abstract class NoParameterStateCondition : IStateCondition
{
	public abstract bool IsSatisfied(IStateInfo stateInfo);

    public bool Parse(params string[] rawParameters)
    {
		return true;
    }
}


public abstract class IntStateCondition : IStateCondition
{
	protected int value;

    public abstract bool IsSatisfied(IStateInfo stateInfo);

    public bool Parse(params string[] rawParameters)
    {
        if (int.TryParse(rawParameters[0], out value))
        {
            return true;
        }

        return false;
    }
}


public class AnimationWaitCondition : FloatParameterStateCondition
{
	public override bool IsSatisfied(IStateInfo stateInfo)
	{
		if (stateInfo is FrameSyncInputMessage animStateInfo)
		{
			return animStateInfo.normalizedTime >= value;
		}

		return false;
	}
}

public class DashCondition : NoParameterStateCondition
{
	public override bool IsSatisfied(IStateInfo stateInfo)
	{
		bool isSatisfied = false;

		if (stateInfo is FrameSyncInputMessage animStateInfo)
		{
			isSatisfied = animStateInfo.isDashInput;
		}

		return isSatisfied;
	}
}

public class PressGuardCondition : NoParameterStateCondition
{
	public override bool IsSatisfied(IStateInfo stateInfo)
	{
		bool isSatisfied = false;

		if (stateInfo is FrameSyncInputMessage animStateInfo)
		{
			isSatisfied = animStateInfo.isGuardInput;
		}

		return isSatisfied;
	}
}

public class GoUpCondition : FloatParameterStateCondition
{
	public override bool IsSatisfied(IStateInfo stateInfo)
	{
        bool isSatisfied = false;

        if (stateInfo is FrameSyncInputMessage animStateInfo)
        {
            isSatisfied = animStateInfo.myEntityVelocity.y > Mathf.Epsilon;
        }

		return isSatisfied;
	}
}

public class PressJumpCondition : NoParameterStateCondition
{
    public override bool IsSatisfied(IStateInfo stateInfo)
    {
		bool isSatisfied = false;

        if (stateInfo is FrameSyncInputMessage animStateInfo)
        {
            isSatisfied = animStateInfo.moveInput.y > Mathf.Epsilon;
        }

        return isSatisfied;
    }
}

public class FallDownCondition : FloatParameterStateCondition
{
	public override bool IsSatisfied(IStateInfo stateInfo)
	{
		bool isSatisfied = false;

		if (stateInfo is FrameSyncInputMessage animStateInfo)
		{
			isSatisfied = animStateInfo.myEntityVelocity.y < -1 * Mathf.Epsilon;
		}

		return isSatisfied;
	}
}

public class MoveCondition : FloatParameterStateCondition
{
	public override bool IsSatisfied(IStateInfo stateInfo)
	{
		bool isSatisfied = false;

		if (stateInfo is FrameSyncInputMessage animStateInfo)
		{
			float currX = animStateInfo.moveInput.x;
			isSatisfied = Mathf.Abs(currX) > Mathf.Abs(value);
		}

		return isSatisfied;
	}
}

public class DamageCondition : IntStateCondition
{
    public override bool IsSatisfied(IStateInfo stateInfo)
    {
        if (stateInfo is FrameSyncInputMessage animStateInfo)
        {
			//EntityManager.Instance.GetEntityComponent()
			//if (animStateInfo.stateParam.attackers == null)
			//	return false;

			//return animStateInfo.stateParam.attackers.Count() > value;
        }

        return false;
    }
}

public class GroundedCondition : IntStateCondition
{
	public override bool IsSatisfied(IStateInfo stateInfo)
	{
		if (stateInfo is FrameSyncInputMessage animStateInfo)
		{
			return animStateInfo.isGrounded == (value > 0);
		}

		return false;
	}
}

public class AttackCondition : IStateCondition
{
	private ENUM_ATTACK_KEY key = ENUM_ATTACK_KEY.NONE;

	public bool IsSatisfied(IStateInfo stateInfo)
	{
		if(stateInfo is FrameSyncInputMessage animStateInfo)
		{
			return animStateInfo.pressedAttackKeyNum == (int)key;
		}

		return false;
	}

	public bool Parse(params string[] rawParameters)
	{
		if(rawParameters.Any() == false)
			return false;

		switch (rawParameters[0])
		{
			case "Normal":
				key = ENUM_ATTACK_KEY.ATTACK;
				return true;

			case "Skill":
				key = ENUM_ATTACK_KEY.SKILL;
				return true;

			case "Ultimate":
				key = ENUM_ATTACK_KEY.ULTIMATE;
				return true;
		}

		return false;
	}
}