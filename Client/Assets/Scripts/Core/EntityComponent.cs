using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityComponent
{
    IEntity Entity
    {
        get;
    }

    void OnUpdate(float deltaTime)
    {

    }
}

/// <summary>
/// 앤티티의 기능을 정의하는 컴포넌트
/// </summary>
public abstract class EntityComponent : MonoBehaviour, IEntityComponent
{
    [SerializeField] private EntityComponentSystem componentSystem;

    private IEntity entity;
	public IEntity Entity
    {
        get
        {
            if (entity == null) 
            {
                entity = GetComponent<EntityBehaviour>();
            }

            return entity;
        }
    }

    protected virtual void Reset()
    {
        componentSystem = SystemHelper.GetSystemAsset<EntityComponentSystem>();
    }

    private void OnEnable()
    {
        componentSystem.Register(this);
    }

    private void OnDisable()
    {
        componentSystem.UnRegister(this);
    }

    public virtual void OnUpdate(float deltaTime)
    {

    }
}