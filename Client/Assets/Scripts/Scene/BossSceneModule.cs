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
		// ���⼭ �̷��� �ϴ� �κ��� �� ���� �Ⱦ... 
		// ���⿡ Damageable, Attackable�� �ְ� ������ async await�� �غ���.
        MapManager.Instance.MoveToMapArea(ENUM_TEAM_TYPE.Friendly, ObjectManager.Instance.PlayerCharacter);
		ObjectManager.Instance.PlayerCharacter.AddComponent<PlayerInput>();
	}

	public override IEnumerator OnPrepareEnterRoutine(SceneModuleParam param)
	{
		yield return MapManager.Instance.LoadAsyncMap(ENUM_MAP_TYPE.City); // �� ����

        yield return ObjectManager.Instance.LoadAsyncPassiveObject(); // ��������, ���� ĳ���Ϳ� �´� Ǯ�� ����

        yield return ObjectManager.Instance.LoadAsyncMonsters(); // ���������� �´� ���� ����
		
        yield return ObjectManager.Instance.LoadAsyncBoss(); // ���������� �´� ���� ����

        yield return ObjectManager.Instance.LoadAsyncPlayer(ENUM_CHARACTER_TYPE.Normal); // ���� ĳ���� ����

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
}
