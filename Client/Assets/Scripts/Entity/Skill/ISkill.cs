using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENUM_SKILL_TYPE
{
    SlashPencil = 0,
    ThrowPencil = 1,
}

public abstract class Skill
{
    private float coolTime;
    private float currentCoolTime;

    protected CharacterComponent owner;

    public void SetOwner(CharacterComponent owner)
    {
        this.owner = owner;
    }

    private bool IsCoolTime()
    {
        return coolTime > currentCoolTime;
    }

    public virtual bool IsSatisfied()
    {
        return IsCoolTime() == false;
    }

    public virtual void OnUpdate(float deltaTime)
    {
        currentCoolTime += deltaTime;
    }

    public virtual void Trigger()
    {
        currentCoolTime = default;
    }
}

public class ThrowSkill : Skill
{
    public override void Trigger()
    {
        base.Trigger();
    }
}

public class ThrowPencil : ThrowSkill
{
    
}
