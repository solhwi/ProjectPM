using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using Unity.VisualScripting;
using UnityEngine;

public static class EntityHelper
{
	public static string ToBehaviourTreePath(this ENUM_ENTITY_TYPE entityType)
	{
		switch (entityType)
		{
			case ENUM_ENTITY_TYPE.PencilMan:
				return "Assets/Bundle/AI/PencilMan.asset";
		}

		return string.Empty;
	}

	public static T GetOrAddComponent<T>(this IEntity entity) where T : MonoBehaviour
	{
		if (entity is EntityBehaviour entityBehaviour)
		{
			return entityBehaviour.gameObject.GetOrAddComponent<T>();
		}

		return null;
	}
}

public class EntityControlSubSystem : MonoSystem
{
	[SerializeField] private AddressableResourceSystem resourceSystem = null;
	[SerializeField] private CommandSystem commandSystem = null;
	[SerializeField] private EntitySystem entitySystem = null;
	[SerializeField] private ScriptParsingSystem scriptParsingSystem = null;

	private IEntity playerEntity = null;
    private Dictionary<int, BehaviourTreeRunner> enemyBehaviourTrees = new Dictionary<int, BehaviourTreeRunner>();

	public static bool IsPlayerControl = true;
	public static bool IsAIControl = true;

    protected override void OnReset()
    {
        base.OnReset();

        resourceSystem = SystemHelper.GetSystemAsset<AddressableResourceSystem>();
		commandSystem = SystemHelper.GetSystemAsset<CommandSystem>();
		entitySystem = SystemHelper.GetSystemAsset<EntitySystem>();
		scriptParsingSystem = SystemHelper.GetSystemAsset<ScriptParsingSystem>();

	}

    public void ToPlayerControl(IEntity entity)
    {
		playerEntity = entity;
	}

	public void ToAIControl(IEntity entity)
	{
		if (entity == null)
			return;

		var runner = entity.GetOrAddComponent<BehaviourTreeRunner>();
		if (runner == null)
			return;

		runner.OnStart(entity, resourceSystem, commandSystem, entitySystem, scriptParsingSystem);
		enemyBehaviourTrees[entity.EntityGuid] = runner;
	}

	private void UpdatePlayerControl()
    {
		if (playerEntity == null)
			return;

		var command = commandSystem.MakeCommand();
		playerEntity.PushCommand(command);
	}

    private void UpdateAIControl()
    {
		foreach (var behaviorTree in enemyBehaviourTrees.Values)
		{
			behaviorTree.OnUpdate();
		}
	}

	public void UpdateControl()
	{
		if(IsPlayerControl)
		{
			UpdatePlayerControl();
		}
		if (IsAIControl)
		{
			UpdateAIControl();
		}
	}
}
