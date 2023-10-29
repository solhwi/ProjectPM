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

	// 여기서 추가 함수로 겹친 의도를 파악합니다.
	// 공격의 의도가 담겨있다면, 공격자가 AttackableComponent가 붙었는 지, 피해자가 DamageableComponent가 붙었는 지 확인합니다.
	// 기본적으로 공격 가능한 Entity는 모두 DamageableComponent가 붙어 있으며, 
	// Entity가 소환한 공격 물체에 AttackableComponent가 붙어 있습니다.

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
