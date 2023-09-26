using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableComponent : MonoBehaviour
{
	public bool isEnable = true;

	EntityMeditatorComponent targetCharacter = null;

	private void Awake()
	{
		targetCharacter = GetComponent<EntityMeditatorComponent>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!isEnable)
			return;

		PhysicsManager.Instance.OnTrigger(collision, this);
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (!isEnable)
			return;

		PhysicsManager.Instance.OnTrigger(collision, this);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (!isEnable)
			return;

		PhysicsManager.Instance.OnTrigger(collision, this);
	}

	public void OnDamage(IEnumerable<AttackableComponent> attackers)
	{
		targetCharacter.OnOtherInput(attackers);
	}
}
