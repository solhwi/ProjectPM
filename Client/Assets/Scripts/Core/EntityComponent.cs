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
/// ��ƼƼ�� ����� �����ϴ� ������Ʈ
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
        componentSystem = AssetLoadHelper.GetSystemAsset<EntityComponentSystem>();
    }

    protected virtual void OnEnable()
    {
        componentSystem.Register(this);
    }

	protected virtual void OnDisable()
    {
        componentSystem.UnRegister(this);
    }

    public virtual void OnUpdate(float deltaTime)
    {

    }
}