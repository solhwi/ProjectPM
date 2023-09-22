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
        MapManager.Instance.MoveToMapArea(ENUM_TEAM_TYPE.Friendly, ObjectManager.Instance.PlayerObject);
	}

	public override IEnumerator OnPrepareEnterRoutine(SceneModuleParam param)
	{
		yield return MapManager.Instance.LoadAsyncMap(ENUM_MAP_TYPE.City); // 맵 생성

        yield return ObjectManager.Instance.LoadAsyncPassiveObject(); // 스테이지, 현재 캐릭터에 맞는 풀링 생성

        yield return ObjectManager.Instance.LoadAsyncMonsters(); // 스테이지에 맞는 몬스터 생성
		
        yield return ObjectManager.Instance.LoadAsyncBoss(); // 스테이지에 맞는 보스 생성

        yield return ObjectManager.Instance.LoadAsyncPlayer(ENUM_CHARACTER_TYPE.RedMan); // 현재 캐릭터 생성

		yield return TimelineManager.Instance.LoadAsyncTimeline(); // 시네마틱 생성
	}

	public override void OnExit()
	{

	}

	public override IEnumerator OnPrepareExitRoutine()
	{
		yield return null;
	}

	public override void OnUpdate(int deltaFrameCount, float deltaTime)
	{
		InputManager.Instance.OnUpdate(deltaFrameCount, deltaTime);
		PhysicsManager.Instance.OnUpdate(deltaFrameCount, deltaTime);
	}

	public override void OnPostUpdate(int deltaFrameCount, float deltaTime)
	{
		ObjectManager.Instance.OnPostUpdate(deltaFrameCount, deltaTime);
	}
    public override void OnLateUpdate(int deltaFrameCount, float deltaTime)
    {
        ObjectManager.Instance.OnLateUpdate(deltaFrameCount, deltaTime);
    }
}
