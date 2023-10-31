using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityComponentSystem : MonoSystem
{
    private List<IEntityComponent> components = new List<IEntityComponent>();

    public void Register(IEntityComponent component)
    {
        if(components.Contains(component) == false)
            components.Add(component);
    }

    public void UnRegister(IEntityComponent component)
    {
        if(components.Contains(component))
            components.Remove(component);
    }

    public override void OnUpdate(int deltaFrameCount, float deltaTime)
    {
        base.OnUpdate(deltaFrameCount, deltaTime);

        foreach(IEntityComponent component in components)
        {
            component.OnUpdate(deltaTime);
        }
    }
}
