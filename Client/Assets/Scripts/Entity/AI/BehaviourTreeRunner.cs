using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class BehaviourTreeRunner : MonoBehaviour
{
	public BehaviourTree tree;

	private Context context;
	private AddressableResourceSystem resourceSystem;

	public void OnStart(IEntity entity, 
		AddressableResourceSystem resourceSystem, 
		CommandSystem commandSystem, 
		EntitySystem entitySystem,
		ScriptParsingSystem scriptParsingSystem)
	{
		this.resourceSystem = resourceSystem;

		context = CreateBehaviourTreeContext();
		context.AddSystem(resourceSystem, commandSystem, entitySystem, scriptParsingSystem);
		
		this.tree = LoadTree(entity.EntityType);
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

	private BehaviourTree LoadTree(ENUM_ENTITY_TYPE entityType)
	{
		string btAssetPath = EntityHelper.ToBehaviourTreePath(entityType);
		return resourceSystem.LoadCached<BehaviourTree>(btAssetPath);
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
