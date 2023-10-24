using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENUM_SKILL_TYPE
{
    None = 0,
    SlashPencil = 1,
    ThrowPencil = 2,
}

public abstract class Skill
{
    private float coolTime;
    private float currentCoolTime;

    protected CharacterBehaviour owner;

    public void SetOwner(CharacterBehaviour owner)
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
