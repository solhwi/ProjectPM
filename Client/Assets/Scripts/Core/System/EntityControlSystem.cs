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
}

public class EntityControlSystem : Singleton<EntityControlSystem>
{
	private AddressableResourceSystem ResourceSystem => AddressableResourceSystem.Instance;

    private CharacterComponent playerCharacter = null;
    private Dictionary<int, BehaviourTreeRunner> enemyBehaviourTrees = new Dictionary<int, BehaviourTreeRunner>();

	public static bool IsPlayerControl = true;
	public static bool IsAIControl = true;
	
    public void ToPlayerControl(CharacterComponent character)
    {
        playerCharacter = character;
	}

	public void ToAIControl(CharacterComponent character)
	{
		if (character == null)
			return;

		var tree = LoadTree(character.EntityType);
		if (tree == null)
			return;

		var runner = character.GetOrAddComponent<BehaviourTreeRunner>();
		if (runner == null)
			return;

		runner.OnStart(tree);
		enemyBehaviourTrees[character.EntityGuid] = runner;
	}

	private void UpdatePlayerControl()
    {
		if (playerCharacter == null)
			return;

		var command = MessageHelper.MakeCommand();
		playerCharacter.SendCommand(command);
	}

    private void UpdateAIControl()
    {
		foreach (var behaviorTree in enemyBehaviourTrees.Values)
		{
			behaviorTree.OnUpdate();
		}
	}

	public override void OnPrevUpdate(int deltaFrameCount, float deltaTime)
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

	private BehaviourTree LoadTree(ENUM_ENTITY_TYPE entityType)
	{
		string btAssetPath = EntityHelper.ToBehaviourTreePath(entityType);
		return ResourceSystem.LoadCached<BehaviourTree>(btAssetPath);
	}
}
