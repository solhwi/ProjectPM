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

public class PhysicsManager : Singleton<PhysicsManager>
{
	private List<PhysicsComponent> physicsComponents = new List<PhysicsComponent>();
	private Queue<GizmoBox> gizmoQueue = new Queue<GizmoBox>();

	public override void OnFixedUpdate(int deltaFrameCount, float deltaTime)
	{
		foreach(var component in physicsComponents)
		{
			component.OnFixedUpdate(deltaFrameCount, deltaTime);
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

}