using NPOI.SS.Formula.Functions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GizmoBox
{
	public readonly Vector3 point;
	public readonly Vector3 size;

	public GizmoBox(Vector3 point, Vector3 size)
	{
		this.point = point;
		this.size = size;
	}
}

public class PhysicsGravitySubSystem
{
    private float gravityPower = 9.8f;
	private float gravityScale = 0.2f;

    private List<PhysicsComponent> physicsComponents = new List<PhysicsComponent>();
	private Dictionary<PhysicsComponent, float> physicsAirborneTimeDictionary = new Dictionary<PhysicsComponent, float>();

	public void OnFixedUpdate(int deltaFrameCount, float deltaTime)
	{
		foreach(var component in physicsComponents)
		{
			if (physicsAirborneTimeDictionary.TryGetValue(component, out float airborneDeltaTime))
			{
				bool isGrounded = component.CheckGrounded();
				if (isGrounded)
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

	public float GetMovementYByGravity(float deltaTime)
	{
		// a * t * t = distance
		return gravityPower * gravityScale * deltaTime * deltaTime;
    }

	public Vector2 GetMovementByGravity(float deltaTime)
	{
		return Vector2.down * GetMovementYByGravity(deltaTime);
    }
}