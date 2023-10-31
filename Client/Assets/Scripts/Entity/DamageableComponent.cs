using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamager
{
    bool IsDamageable(IEntity attacker);

    void TakeDamage(ENUM_SKILL_TYPE attackType, IEntity attacker);
}

[RequireComponent(typeof(Collider2D))]
public class DamageableComponent : EntityComponent, IDamager
{
	public bool Invincible = false;

	public bool IsDamageable(IEntity attacker)
	{
		if (attacker.IsAttackable == false)
			return false;

		if (attacker.OwnerGuid == Entity.OwnerGuid)
			return false;

		return Invincible == false;
	}

	public void TakeDamage(ENUM_SKILL_TYPE attackType, IEntity attacker)
    {

    }
}
