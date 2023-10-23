using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollisionSubSystem
{
    private EntitySystem entitySystem => EntitySystem.Instance;

	private readonly Queue<GizmoBox> gizmoQueue = new Queue<GizmoBox>();
	private List<PhysicsComponent> physicsComponents = new List<PhysicsComponent>();

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

	public void OnFixedUpdate(int deltaFrameCount, float deltaTime)
	{
		foreach (var component in physicsComponents)
		{
			component.UpdateRaycastHit();
		}
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
			Gizmos.color = UnityEngine.Color.red;
			Gizmos.DrawWireCube(gizmoBox.point, gizmoBox.size);
		}
	}

	private IEnumerable<EntityComponent> GetOverlapEntities(int entityGuid, Vector3 pos, Vector3 size, Vector3 offset, Vector3 velocity, bool includeMine = false)
	{
		Collider2D[] colliders = OverlapBoxAll(pos + offset + velocity, size);
		if (colliders == null || colliders.Length == 0)
			yield break;

		var myEntity = entitySystem.GetEntityComponent(entityGuid);
		if (myEntity == null)
			yield break;

		foreach (var collider in colliders)
		{
			var entity = collider.GetComponent<EntityComponent>();
			if (entity == null)
				continue;

			if (includeMine == false && entity.OwnerGuid == myEntity.OwnerGuid)
				continue;

			yield return entity;
		}
	}

	private IEnumerable<EntityComponent> GetOverlapEntities(FrameEntityMessage message, bool includeMine)
	{
		return GetOverlapEntities(message.entityGuid, message.pos, message.hitbox, message.offset, message.velocity, includeMine);
	}

	public IEnumerable<EntityComponent> GetSearchedEntities(EntityComponent entity, ENUM_SKILL_TYPE skillType, bool includeMine = false)
	{
		var skillTable = ScriptParsingSystem.Instance.GetTable<CharacterSkillTable>();
		if (skillTable == null)
			return null;

		var hasSkill = skillTable.GetSkillInfo(skillType);
		if (hasSkill == null)
			return null;

		Vector2 box = new(hasSkill.searchBoxX, hasSkill.searchBoxY);
		Vector2 offset = new(hasSkill.searchOffsetX, hasSkill.searchOffsetY);

		return GetOverlapEntities(entity.EntityGuid, entity.Position, box, offset, entity.Velocity, includeMine);
	}

	public IEnumerable<EntityComponent> GetOverlapEntities(EntityComponent entity, bool includeMine = false)
	{
		return GetOverlapEntities(entity.EntityGuid, entity.Position, entity.HitBox, entity.HitOffset, entity.Velocity, includeMine);
	}

	public IEnumerable<int> GetOverlapEntitiyGuids(FrameEntityMessage message, bool includeMine = false)
	{
		return GetOverlapEntities(message, includeMine).Select(entity => entity.EntityGuid);
	}

	public float GetXDistance(EntityComponent fromEntity, EntityComponent toEntity)
	{
		return toEntity.Position.x - fromEntity.Position.x;
	}

	public float GetXDistanceFromPlayer(EntityComponent fromEntity)
	{
		return GetXDistance(fromEntity, entitySystem.PlayerCharacter);
	}

	public float GetXDistanceFromBoss(EntityComponent fromEntity)
	{
		return GetXDistance(fromEntity, entitySystem.BossCharacter);
	}

}
