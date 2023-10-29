using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamager
{
    bool IsDamageable 
    { 
        get;
    }

    void TakeDamage(ENUM_SKILL_TYPE attackType, IAttacker attacker);
}

[RequireComponent(typeof(Collider2D))]
public class DamageableComponent : MonoComponent, IDamager
{
    public ENUM_LAYER_TYPE TeamType;
    public bool IsInvinsible = false;

    public void TakeDamage(ENUM_SKILL_TYPE attackType, IAttacker attacker)
    {

    }
}
