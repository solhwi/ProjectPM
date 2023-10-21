using Mirror;
using Mirror.Examples.CCU;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public struct EnemySpawnData
{
	public float spawnTime;
    public ENUM_ENTITY_TYPE entityType;
}

public class BossSceneModuleParam : SceneModuleParam
{
	public readonly ENUM_MAP_TYPE mapType;
	public readonly ENUM_ENTITY_TYPE playerType;

	public BossSceneModuleParam(ENUM_MAP_TYPE mapType, ENUM_ENTITY_TYPE playerType)
	{
		this.mapType = mapType;
		this.playerType = playerType;
	}
}

public class BossSceneModule : SceneModule
{
    public override void OnEnter(SceneModuleParam param)
	{
		foreach (var monster in EntityManager.Instance.Enemies)
		{
			MapManager.Instance.MoveToSafeArea(monster);
		}

		MapManager.Instance.MoveToMapArea(ENUM_TEAM_TYPE.Friendly, EntityManager.Instance.PlayerCharacter);
        CharacterController.Instance.RegisterControl(EntityManager.Instance.PlayerCharacter);
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
        yield return EntityManager.Instance.LoadAsyncBoss(StageManager.Instance.GetCurrentStageBoss()); // 몬스터 생성
        yield return EntityManager.Instance.LoadAsyncEnemies(StageManager.Instance.GetCurrentStageEnemies()); // 적 생성
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
		base.OnUpdate(deltaFrameCount, deltaTime);

        PhysicsManager.Instance.OnUpdate(deltaFrameCount, deltaTime);
        EntityManager.Instance.OnUpdate(deltaFrameCount, deltaTime);

        OnUpdateMonsters();
    }

    protected override void OnDrawGizmos()
    {
		PhysicsManager.Instance.OnDrawGizmos();
    }

	private void OnUpdateMonsters()
	{
		var enemySpawnDataQueue = new Queue<EnemySpawnData>();

		foreach (var spawnData in StageManager.Instance.GetCurrentStageEnemies())
		{
			enemySpawnDataQueue.Enqueue(spawnData);
		}

		enemySpawnDataQueue.Enqueue(StageManager.Instance.GetCurrentStageBoss());

		while(true)
		{
			if (enemySpawnDataQueue.TryPeek(out var spawnData) == false)
				break;

			if (spawnData.spawnTime > sceneOpenDeltaTime)
				break;

            var enemy = EntityManager.Instance.GetEnemy(spawnData.entityType);
            if (enemy != null)
			{
				MapManager.Instance.MoveToMapArea(ENUM_TEAM_TYPE.Enemy, enemy);
				CharacterController.Instance.RegisterAI(enemy);
			}

            enemySpawnDataQueue.Dequeue();
        }
	}
}
