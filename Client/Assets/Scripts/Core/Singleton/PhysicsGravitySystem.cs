using NPOI.SS.Formula.Functions;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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

public class PhysicsGravitySystem : Singleton<PhysicsGravitySystem>
{
    private float gravityPower = 9.8f;
	private float gravityScale = 0.2f;

    private List<PhysicsComponent> physicsComponents = new List<PhysicsComponent>();
	private readonly Queue<GizmoBox> gizmoQueue = new Queue<GizmoBox>();

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

	public Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size)
	{
		gizmoQueue.Enqueue(new GizmoBox(point, size));
        return Physics2D.OverlapBoxAll(point, size, 0);
    }

    public void OnDrawGizmos()
    {
		while(gizmoQueue.TryDequeue(out GizmoBox gizmoBox))
		{
            Gizmos.color = UnityEngine.Color.red;
            Gizmos.DrawWireCube(gizmoBox.point, gizmoBox.size);
        }
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