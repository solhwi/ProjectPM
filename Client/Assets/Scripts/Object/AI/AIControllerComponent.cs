using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControllerComponent : MonoBehaviour
{
	public bool isEnable = true;

	private Blackboard blackBoard;
	private MonoBehaviourTree tree;

	private CharacterComponent characterComponent = null;

	private void Awake()
	{
		blackBoard = GetComponent<Blackboard>();
		tree = GetComponent<MonoBehaviourTree>();

		characterComponent = GetComponent<CharacterComponent>();
	}

	void Update()
	{
		tree.Tick();
	}
}
