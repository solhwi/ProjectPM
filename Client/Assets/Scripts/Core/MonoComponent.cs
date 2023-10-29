using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonoComponent
{
    IEntity Entity
    {
        get;
    }
}

/// <summary>
/// ��ƼƼ�� ����� �����ϴ� ������Ʈ
/// </summary>
public abstract class MonoComponent : MonoBehaviour, IMonoComponent
{
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
}