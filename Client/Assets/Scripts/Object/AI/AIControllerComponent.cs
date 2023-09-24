using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIType
{
	Dummy = 0,
}

[RequireComponent(typeof(CharacterComponent))]
[RequireComponent(typeof(Blackboard))]
[RequireComponent(typeof(MonoBehaviourTree))]
public class AIControllerComponent : MonoBehaviour
{
	public bool isEnable = true;

	private MonoBehaviourTree tree;

	public void Initialize()
	{
		tree = GetComponent<MonoBehaviourTree>();
	}

	private void Update()
	{
		if (isEnable)
		{
			tree.Tick();
		}
	}
}
