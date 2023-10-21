using Mirror.Examples.CCU;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossSceneModuleParam : SceneModuleParam
{
	public readonly ENUM_MAP_TYPE mapType;
	public readonly ENUM_ENTITY_TYPE playerType;
	public readonly ENUM_ENTITY_TYPE bossType;
	public readonly IEnumerable<ENUM_ENTITY_TYPE> enemyTypes;

	public BossSceneModuleParam(ENUM_MAP_TYPE mapType, ENUM_ENTITY_TYPE playerType, ENUM_ENTITY_TYPE bossType, params ENUM_ENTITY_TYPE[] enemyTypes)
	{
		this.mapType = mapType;
		this.playerType = playerType;
		this.bossType = bossType;
		this.enemyTypes = enemyTypes;
	}
}

public class BossSceneModule : SceneModule
{
    public override void OnEnter(SceneModuleParam param)
	{
        MapManager.Instance.MoveToMapArea(ENUM_TEAM_TYPE.Friendly, EntityManager.Instance.PlayerCharacter);
        CharacterController.Instance.RegisterControl(EntityManager.Instance.PlayerCharacter);

        MapManager.Instance.MoveToMapArea(ENUM_TEAM_TYPE.Enemy, EntityManager.Instance.BossCharacter);
        CharacterController.Instance.RegisterAI(EntityManager.Instance.BossCharacter);

        foreach (var monster in EntityManager.Instance.Monsters)
		{
            MapManager.Instance.MoveToMapArea(ENUM_TEAM_TYPE.Enemy, monster);
            CharacterController.Instance.RegisterAI(monster);
        }

    }

	public override void OnExit()
	{
		CharacterController.Instance.UnRegisterControl();
	}

	public override IEnumerator OnPrepareEnterRoutine(SceneModuleParam param)
	{
		var _param = param as BossSceneModuleParam;
		if (_param == null)
		{
			Debug.LogError("���� �� ��⿡ �����ϱ� ���� �Ķ���Ͱ� �̻��մϴ�.");
			yield break;
		}

        yield return MapManager.Instance.LoadAsyncMap(_param.mapType); // �� ����
        yield return EntityManager.Instance.LoadAsyncBoss(_param.bossType); // ���� ����
        yield return EntityManager.Instance.LoadAsyncMonsters(_param.enemyTypes); // ���� ����
        yield return EntityManager.Instance.LoadAsyncPlayer(_param.playerType); // �÷��̾� ����

    }

    public override void OnFixedUpdate(int tickCount, float latencyTime)
	{
		PhysicsManager.Instance.OnFixedUpdate(tickCount, latencyTime);
	}

	public override void OnPrevUpdate(int deltaFrameCount, float deltaTime)
	{
		CharacterController.Instance.OnPrevUpdate(deltaFrameCount, deltaTime);
	}

	public override void OnUpdate(int deltaFrameCount, float deltaTime)
	{
        PhysicsManager.Instance.OnUpdate(deltaFrameCount, deltaTime);
        EntityManager.Instance.OnUpdate(deltaFrameCount, deltaTime);
	}

    protected override void OnDrawGizmos()
    {
		PhysicsManager.Instance.OnDrawGizmos();
    }
}
