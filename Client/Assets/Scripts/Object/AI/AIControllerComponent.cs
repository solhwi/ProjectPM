using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Blackboard))]
[RequireComponent(typeof(MonoBehaviourTree))]
public class AIControllerComponent : MonoBehaviour
{
	public bool isEnable = true;

	private MonoBehaviourTree tree;
	private CharacterComponent characterComponent = null;

	private void Awake()
	{
		tree = GetComponent<MonoBehaviourTree>();
		characterComponent = GetComponent<CharacterComponent>();
	}

	private void Update()
	{
		if (isEnable)
		{
			tree.Tick();
		}
	}
}
