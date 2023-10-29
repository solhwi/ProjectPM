using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonoComponent
{
    void SetOwner(IEntity entity);
}

public abstract class MonoComponent : MonoBehaviour, IMonoComponent
{
    [SerializeField] protected IEntity owner;

    protected virtual void Reset()
    {
        var myEntity = GetComponent<IEntity>();
        SetOwner(owner);
    }

    public void SetOwner(IEntity entity)
    {
        this.entity = entity;
    }
}