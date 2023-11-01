using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileComponent : EntityComponent
{
    [SerializeField] RenderingComponent renderingComponent;
    [SerializeField] PhysicsComponent physicsComponent;

	protected override void Reset()
	{
		base.Reset();

		renderingComponent = GetComponent<RenderingComponent>();
		physicsComponent = GetComponent<PhysicsComponent>();
	}

	protected override void OnEnable()
	{
		base.OnEnable();

        renderingComponent.Initialize(ENUM_LAYER_TYPE.Projectile, Entity.EntityGuid);
	}

	public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        physicsComponent.AddMovement(Vector2.left * deltaTime);

		bool isLeft = physicsComponent.Velocity.x < Mathf.Epsilon;
		renderingComponent.Look(isLeft);
    }
}
