using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackableComponent : MonoBehaviour
{
	private Collider2D myCollider = null;
	public EntityComponent AttackOwner { get; private set; }

	public void Initialize(EntityComponent owner)
	{
		AttackOwner = owner;
		myCollider = GetComponent<Collider2D>();
	}

	public void RegisterCollider()
    {
        PhysicsManager.Instance.RegisterCollider(myCollider, this);
    }

    public void UnRegisterCollider()
    {
		PhysicsManager.Instance.UnRegisterCollider(myCollider);
	}
}
