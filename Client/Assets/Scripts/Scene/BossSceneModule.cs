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
		ControllerManager.Instance.SetPlayerController(EntityManager.Instance.PlayerEntity);

		Application.targetFrameRate = 30;
    }

	public override IEnumerator OnPrepareEnterRoutine(SceneModuleParam param)
	{
		yield return MapManager.Instance.LoadAsyncMap(ENUM_MAP_TYPE.City); // �� ����

        yield return EntityManager.Instance.LoadAsyncPassiveObject(); // ��������, ���� ĳ���Ϳ� �´� Ǯ�� ����

        yield return EntityManager.Instance.LoadAsyncMonsters(); // ���������� �´� ���� ����
		
        yield return EntityManager.Instance.LoadAsyncBoss(); // ���������� �´� ���� ����

        yield return EntityManager.Instance.LoadAsyncPlayer(ENUM_ENTITY_TYPE.RedMan); // ���� ĳ���� ����

		yield return TimelineManager.Instance.LoadAsyncTimeline(); // �ó׸�ƽ ����
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
		EntityManager.Instance.OnPostUpdate(deltaFrameCount, deltaTime);
		OfflineBattleManager.Instance.OnPostUpdate(deltaFrameCount, deltaTime);
	}

	public override void OnLateUpdate(int deltaFrameCount, float deltaTime)
    {
        EntityManager.Instance.OnLateUpdate(deltaFrameCount, deltaTime);
    }
}
