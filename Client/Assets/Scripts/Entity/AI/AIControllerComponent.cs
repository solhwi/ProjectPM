using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENUM_AI_TYPE
{
	NormalMonster = 0,
}

// [RequireComponent(typeof(Blackboard))]
// [RequireComponent(typeof(MonoBehaviourTree))]
public class AIControllerComponent : MonoBehaviour
{
	private ENUM_AI_TYPE aiType;
    private MonoBehaviourTree monoBehaviourTree;

	public bool isEnable = false;

    private FrameInputSnapShotMessage snapShotMessage = new FrameInputSnapShotMessage();

    public virtual void Initialize(ENUM_AI_TYPE aiType)
	{
		this.aiType = aiType;
        monoBehaviourTree = GetComponent<MonoBehaviourTree>();
        // ai 타입에 맞게 MonoBehaviour 트리를 프리팹에서 로드한다.
    }

    private void Update()
    {
		if (isEnable == false)
			return;

		monoBehaviourTree.Tick();
    }
}
