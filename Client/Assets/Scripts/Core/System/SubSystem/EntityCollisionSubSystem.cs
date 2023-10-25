using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityCollisionSubSystem : MonoSystem
{
	[SerializeField] private EntitySystem entitySystem;

	private Queue<GizmoBox> gizmoQueue = new Queue<GizmoBox>();

    protected override void OnReset()
    {
        base.OnReset();

        entitySystem = SystemHelper.GetSystemAsset<EntitySystem>();
    }

    private Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size)
	{
		gizmoQueue.Enqueue(new GizmoBox(point, size));
		return Physics2D.OverlapBoxAll(point, size, 0);
	}

	public void OnDrawGizmos()
	{
		while (gizmoQueue.TryDequeue(out GizmoBox gizmoBox))
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(gizmoBox.point, gizmoBox.size);
		}
	}

	public IEnumerable<IEntity> GetOverlapEntities(int entityGuid, Vector3 pos, Vector3 size, Vector3 offset, Vector3 velocity, bool includeMine)
	{
		Collider2D[] colliders = OverlapBoxAll(pos + offset + velocity, size);
		if (colliders == null || colliders.Length == 0)
			yield break;

		var myEntity = entitySystem.GetEntity(entityGuid);
		if (myEntity == null)
			yield break;

		foreach (var collider in colliders)
		{
			var entity = collider.GetComponent<IEntity>();
			if (entity == null)
				continue;

			if (includeMine == false && entity.OwnerGuid == myEntity.OwnerGuid)
				continue;

			yield return entity;
		}
	}
}
