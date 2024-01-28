using NPOI.SS.Formula.Functions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsGravitySubSystem : MonoSystem
{
	private List<PhysicsComponent> physicsComponents = new List<PhysicsComponent>();
	private Dictionary<PhysicsComponent, float> physicsAirborneTimeDictionary = new Dictionary<PhysicsComponent, float>();

    public override void OnUpdate(int deltaFrameCount, float deltaTime)
	{
		foreach(var component in physicsComponents)
		{
			if (physicsAirborneTimeDictionary.TryGetValue(component, out float airborneDeltaTime))
			{
				bool isGrounded = component.CheckGrounded();
				bool useGravity = component.UseGravity;
				if (isGrounded || !useGravity)
				{
					// 여기에 y축 위치를 초기화하는 코드가 필요합니다.

					physicsAirborneTimeDictionary[component] = 0.0f;
				}
				else
				{
					var gravityMovement = GetMovementByGravity(airborneDeltaTime);
					component.AddMovement(gravityMovement);

					physicsAirborneTimeDictionary[component] += deltaTime;
				}
			}
		}
	}

    public void Register(PhysicsComponent physicsComponent)
	{
		if (physicsComponents.Contains(physicsComponent))
			return;

		physicsComponents.Add(physicsComponent);
		physicsAirborneTimeDictionary.Add(physicsComponent, 0.0f);
	}

	public void UnRegister(PhysicsComponent physicsComponent)
	{
		if (physicsComponents.Contains(physicsComponent) == false)
			return;

		physicsComponents.Remove(physicsComponent);
		physicsAirborneTimeDictionary.Remove(physicsComponent);
	}

	private float GetMovementYByGravity(float deltaTime)
	{
		// a * t * t = distance
		return 9.8f * deltaTime * deltaTime;
	}

	private Vector2 GetMovementByGravity(float deltaTime)
	{
		return Vector2.down * GetMovementYByGravity(deltaTime);
	}
}