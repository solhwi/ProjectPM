using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsManager : Singleton<PhysicsManager>
{
	private List<PhysicsComponent> physicsComponents = new List<PhysicsComponent>();

	public override void OnFixedUpdate(int deltaFrameCount, float deltaTime)
	{
		foreach(var component in physicsComponents)
		{
			component.OnFixedUpdate(deltaFrameCount, deltaTime);
		}
	}

	public override void OnUpdate(int deltaFrameCount, float deltaTime)
	{
		foreach (var component in physicsComponents)
		{
			component.OnUpdate(deltaFrameCount, deltaTime);
		}
	}

	public void Register(PhysicsComponent physicsComponent)
	{
		if (physicsComponents.Contains(physicsComponent))
			return;

		physicsComponents.Add(physicsComponent);
	}

	public void UnRegister(PhysicsComponent physicsComponent)
	{
		if (physicsComponents.Contains(physicsComponent) == false)
			return;

		physicsComponents.Remove(physicsComponent);
	}
}