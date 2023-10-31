using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : EntityComponent
{
    [SerializeField] PhysicsComponent physicsComponent;

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        physicsComponent.AddMovement(Vector2.left * deltaTime);
    }
}
