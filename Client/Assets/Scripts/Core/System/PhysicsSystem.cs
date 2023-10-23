using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSystem : Singleton<PhysicsSystem>
{
	public static PhysicsGravitySubSystem Gravity = new PhysicsGravitySubSystem();
	public static CollisionSubSystem Collision = new CollisionSubSystem();

	List<PhysicsComponent> physicsComponents = new List<PhysicsComponent>();

	public void Register(PhysicsComponent physicsComponent)
	{
		if (physicsComponents.Contains(physicsComponent))
			return;

		physicsComponents.Add(physicsComponent);

		Collision.Register(physicsComponent);
		Gravity.Register(physicsComponent);
	}

	public void UnRegister(PhysicsComponent physicsComponent)
	{
		if (physicsComponents.Contains(physicsComponent) == false)
			return;

		physicsComponents.Remove(physicsComponent);

		Collision.UnRegister(physicsComponent);
		Gravity.UnRegister(physicsComponent);
	}

	public override void OnFixedUpdate(int deltaFrameCount, float deltaTime)
	{
		Collision.OnFixedUpdate(deltaFrameCount, deltaTime);
		Gravity.OnFixedUpdate(deltaFrameCount, deltaTime);

		foreach (var component in physicsComponents)
		{
			component.FlushMovement();
		}
	}

	public void OnDrawGizmos()
	{
		Collision.OnDrawGizmos();
	}
}
