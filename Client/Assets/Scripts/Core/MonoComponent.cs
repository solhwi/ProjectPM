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
/// 앤티티의 기능을 정의하는 컴포넌트
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