using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsHelper
{
	public static float GetMovementYByGravity(float deltaTime)
	{
		// a * t * t = distance
		return PhysicsConfig.GravityPower * PhysicsConfig.GravityScale * deltaTime * deltaTime;
	}

	public static Vector2 GetMovementByGravity(float deltaTime)
	{
		return Vector2.down * GetMovementYByGravity(deltaTime);
	}
}

public class PhysicsSystem : MonoSystem
{
	[SerializeField] private PhysicsGravitySubSystem gravitySubSystem;
	private List<PhysicsComponent> physicsComponents = new List<PhysicsComponent>();

    protected override void OnReset()
    {
        base.OnReset();

        gravitySubSystem = SystemHelper.GetSystemAsset<PhysicsGravitySubSystem>();
    }

    public void Register(PhysicsComponent physicsComponent)
	{
		if (physicsComponents.Contains(physicsComponent))
			return;

		physicsComponents.Add(physicsComponent);
		gravitySubSystem.Register(physicsComponent);
	}

	public void UnRegister(PhysicsComponent physicsComponent)
	{
		if (physicsComponents.Contains(physicsComponent) == false)
			return;

		physicsComponents.Remove(physicsComponent);
		gravitySubSystem.UnRegister(physicsComponent);
	}

	public override void OnUpdate(int deltaFrameCount, float deltaTime)
	{
		foreach (var component in physicsComponents)
		{
			component.UpdateRaycastHit();
		}

		gravitySubSystem.OnUpdate(deltaFrameCount, deltaTime);

		foreach (var component in physicsComponents)
		{
			component.FlushMovement();
		}
	}
}
