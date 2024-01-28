using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class BehaviourTreeRunner : MonoBehaviour
{
	public BehaviourTree tree;

	private Context context;
	private AddressableResourceSystem resourceSystem;

	public void OnStart(AddressableResourceSystem resourceSystem, 
		ScriptParsingSystem scriptParsingSystem)
	{
		this.resourceSystem = resourceSystem;

		context = CreateBehaviourTreeContext();
		context.AddSystem(resourceSystem, scriptParsingSystem);
		
		this.tree = LoadTree();
		this.tree.Bind(context);
	}

	public void OnUpdate()
	{
		if (tree)
		{
			tree.Update();
		}
	}

	private Context CreateBehaviourTreeContext()
	{
		return Context.CreateFromGameObject(gameObject);
	}

	private BehaviourTree LoadTree()
	{
		return resourceSystem.LoadCached<BehaviourTree>("");
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
