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
		yield return MapManager.Instance.LoadAsyncMap(ENUM_MAP_TYPE.City); // �� ����

        yield return ObjectManager.Instance.LoadAsyncPassiveObject(); // ��������, ���� ĳ���Ϳ� �´� Ǯ�� ����

        yield return ObjectManager.Instance.LoadAsyncMonsters(); // ���������� �´� ���� ����
		
        yield return ObjectManager.Instance.LoadAsyncBoss(); // ���������� �´� ���� ����

        yield return ObjectManager.Instance.LoadAsyncPlayer(ENUM_CHARACTER_TYPE.RedMan); // ���� ĳ���� ����

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
		ObjectManager.Instance.OnPostUpdate(deltaFrameCount, deltaTime);
	}
    public override void OnLateUpdate(int deltaFrameCount, float deltaTime)
    {
        ObjectManager.Instance.OnLateUpdate(deltaFrameCount, deltaTime);
    }
}
