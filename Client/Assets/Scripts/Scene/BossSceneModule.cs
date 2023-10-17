using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossSceneModuleParam : SceneModuleParam
{
	public readonly ENUM_MAP_TYPE mapType;

	public BossSceneModuleParam(ENUM_MAP_TYPE mapType)
	{
		this.mapType = mapType;
	}
}

public class BossSceneModule : SceneModule
{
    public override void OnEnter(SceneModuleParam param)
	{
        MapManager.Instance.MoveToMapArea(ENUM_TEAM_TYPE.Friendly, EntityManager.Instance.PlayerEntity);
        MapManager.Instance.MoveToMapArea(ENUM_TEAM_TYPE.Enemy, EntityManager.Instance.MonsterEntity);

        CharacterController.Instance.RegisterControl(EntityManager.Instance.PlayerEntity);
		CharacterController.Instance.RegisterAI(EntityManager.Instance.MonsterEntity);
    }

	public override void OnExit()
	{
		CharacterController.Instance.UnRegisterControl();
	}

	public override IEnumerator OnPrepareEnterRoutine(SceneModuleParam param)
	{
		yield return MapManager.Instance.LoadAsyncMap(ENUM_MAP_TYPE.City); // 맵 생성
        yield return EntityManager.Instance.LoadAsyncPlayer(ENUM_ENTITY_TYPE.RedMan); // 현재 캐릭터 생성
		yield return EntityManager.Instance.LoadAsyncMonster(ENUM_ENTITY_TYPE.PencilMan); // 현재 몬스터 생성
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
		EntityManager.Instance.OnUpdate(deltaFrameCount, deltaTime);
	}
}
