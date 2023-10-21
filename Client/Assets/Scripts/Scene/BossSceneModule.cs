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
			Debug.LogError("보스 씬 모듈에 진입하기 위한 파라미터가 이상합니다.");
			yield break;
		}

        yield return MapManager.Instance.LoadAsyncMap(_param.mapType); // 맵 생성
        yield return EntityManager.Instance.LoadAsyncBoss(_param.bossType); // 몬스터 생성
        yield return EntityManager.Instance.LoadAsyncMonsters(_param.enemyTypes); // 몬스터 생성
        yield return EntityManager.Instance.LoadAsyncPlayer(_param.playerType); // 플레이어 생성

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
