using System.Linq;
using UnityEngine;

public interface IStateCondition
{
	public bool Parse(params string[] rawParameters);
	public bool IsSatisfied(FrameInputSnapShotMessage stateInfo);
}

public abstract class FloatParameterStateCondition : IStateCondition
{
    protected float value = 0.0f;

	public abstract bool IsSatisfied(FrameInputSnapShotMessage stateInfo);

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
	public abstract bool IsSatisfied(FrameInputSnapShotMessage stateInfo);

    public bool Parse(params string[] rawParameters)
    {
		return true;
    }
}


public abstract class IntStateCondition : IStateCondition
{
	protected int value;

    public abstract bool IsSatisfied(FrameInputSnapShotMessage stateInfo);

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
	public override bool IsSatisfied(FrameInputSnapShotMessage stateInfo)
	{
		var entityStateInfo = stateInfo.ConvertToAnimationMessage();
		return entityStateInfo.normalizedTime >= value;
	}
}

public class DashCondition : NoParameterStateCondition
{
	public override bool IsSatisfied(FrameInputSnapShotMessage stateInfo)
	{
		var entityStateInfo = stateInfo.ConvertToInput();
		return entityStateInfo.isDash;
	}
}

public class PressGuardCondition : NoParameterStateCondition
{
	public override bool IsSatisfied(FrameInputSnapShotMessage stateInfo)
	{
		var entityStateInfo = stateInfo.ConvertToInput();
		return entityStateInfo.isGuard;
	}
}

public class GoUpCondition : FloatParameterStateCondition
{
	public override bool IsSatisfied(FrameInputSnapShotMessage stateInfo)
	{
		var entityStateInfo = stateInfo.ConvertToEntity();
		return entityStateInfo.myEntityVelocity.y < -1 * Mathf.Epsilon;
	}
}

public class PressJumpCondition : NoParameterStateCondition
{
    public override bool IsSatisfied(FrameInputSnapShotMessage stateInfo)
    {
		var entityStateInfo = stateInfo.ConvertToInput();
		return entityStateInfo.moveInput.y < -1 * Mathf.Epsilon;
    }
}

public class FallDownCondition : FloatParameterStateCondition
{
	public override bool IsSatisfied(FrameInputSnapShotMessage stateInfo)
	{
		var entityStateInfo = stateInfo.ConvertToEntity();
		return entityStateInfo.myEntityVelocity.y < -1 * Mathf.Epsilon;
	}
}

public class MoveCondition : FloatParameterStateCondition
{
	public override bool IsSatisfied(FrameInputSnapShotMessage stateInfo)
	{
		var entityStateInfo = stateInfo.ConvertToInput();
		float currX = entityStateInfo.moveInput.x;
		return Mathf.Abs(currX) > Mathf.Abs(value);
	}
}

public class DamageCondition : IntStateCondition
{
    public override bool IsSatisfied(FrameInputSnapShotMessage stateInfo)
    {
		var entityStateInfo = stateInfo.ConvertToEntity();
		if (entityStateInfo.attackerEntities == null)
			return false;

		return entityStateInfo.attackerEntities.Count() > value;
    }
}

public class GroundedCondition : IntStateCondition
{
	public override bool IsSatisfied(FrameInputSnapShotMessage stateInfo)
	{
		var entityStateInfo = stateInfo.ConvertToEntity();
		return entityStateInfo.isGrounded == (value > 0);
	}
}

public class AttackCondition : IStateCondition
{
	private ENUM_ATTACK_KEY key = ENUM_ATTACK_KEY.NONE;

	public bool IsSatisfied(FrameInputSnapShotMessage stateInfo)
	{
		var entityStateInfo = stateInfo.ConvertToInput();
		return entityStateInfo.pressedAttackKeyNum == (int)key;
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