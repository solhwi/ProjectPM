using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableComponent : MonoBehaviour
{
	ObjectComponent targetObject = null;

	private void Awake()
	{
		targetObject = GetComponent<ObjectComponent>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		PhysicsManager.Instance.OnTrigger(collision, this);
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		PhysicsManager.Instance.OnTrigger(collision, this);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		PhysicsManager.Instance.OnTrigger(collision, this);
	}

	public void OnDamage(IEnumerable<AttackableComponent> attackers)
	{
		targetObject.OnDamageInput(attackers);
	}
}
