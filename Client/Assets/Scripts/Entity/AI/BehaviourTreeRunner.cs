using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class BehaviourTreeRunner : MonoBehaviour
{
	public BehaviourTree tree;
	private Context context;

	public void OnStart(BehaviourTree tree)
	{
		context = CreateBehaviourTreeContext();
		this.tree = tree.Clone();
		this.tree.Bind(context);
	}

	public void OnUpdate()
	{
		if (tree)
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
