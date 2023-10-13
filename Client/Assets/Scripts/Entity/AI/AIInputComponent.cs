using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public enum ENUM_AI_TYPE
{
	PencilMan = 0,
}

public class AIInputComponent : InputComponent
{
	public ENUM_AI_TYPE AIType
	{
		get
		{
			return aiType;
		}
	}
	private ENUM_AI_TYPE aiType;


	// The main behaviour tree asset
	public BehaviourTree tree;

	// Storage container object to hold game object subsystems
	Context context;

	public override void Run()
	{
		context = CreateBehaviourTreeContext();
		tree = tree.Clone();
		tree.Bind(context);
	}

	public override void OnPrevUpdate(int deltaFrameCount, float deltaTime)
	{
		if (tree && isEnable)
		{
			tree.Update();
		}
	}

	Context CreateBehaviourTreeContext()
	{
		return Context.CreateFromGameObject(gameObject);
	}

	private void OnDrawGizmosSelected()
	{
		if (!tree)
			return;

		BehaviourTree.Traverse(tree.rootNode, (n) =>
		{
			if (n.drawGizmos)
			{
				n.OnDrawGizmos();
			}
		});
	}
}
