using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackableEntityBehaviour : EntityBehaviour
{
	public override bool IsAttackable => true;
}
