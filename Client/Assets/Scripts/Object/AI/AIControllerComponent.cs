using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENUM_AI_TYPE
{
	NormalMonster = 0,
}

[RequireComponent(typeof(Blackboard))]
[RequireComponent(typeof(MonoBehaviourTree))]
public abstract class AIControllerComponent : MonoBehaviour
{
	public abstract ENUM_AI_TYPE AIType
	{
		get;
	}

    private MBTExecutor executor = null;

    private void Reset()
    {
		Initialize();
    }

    public virtual void Initialize()
	{
		executor = gameObject.GetOrAddComponent<MBTExecutor>();
	}

	public void Activate()
	{
		executor.isEnable = true;
    }

	public void DeActivate()
	{
		executor.isEnable = false;
	}
}
