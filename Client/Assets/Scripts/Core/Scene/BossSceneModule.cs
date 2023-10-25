using Cysharp.Threading.Tasks;
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

public class BossSceneModule : BattleSceneModule
{

    public override void OnEnter(SceneModuleParam param)
	{
		mapSystem.Spawn(entitySystem.Player);
        entitySystem.ToPlayerControl();
	}

	public override void OnExit()
	{
		
	}

	public async override UniTask OnPrepareEnterRoutine(SceneModuleParam param)
	{
		var _param = param as BossSceneModuleParam;
		if (_param == null)
		{
			Debug.LogError("���� �� ��⿡ �����ϱ� ���� �Ķ���Ͱ� �̻��մϴ�.");
			return;	
		}

        await mapSystem.CreateMap(_param.mapType); // �� ����
        await entitySystem.CreatePlayer(_param.playerType); // �÷��̾� ����
    }

	public override void OnFixedUpdate(int tickCount, float latencyTime)
	{
		physicsSystem.OnUpdate(tickCount, latencyTime);
	}

	public override void OnPrevUpdate(int deltaFrameCount, float deltaTime)
	{
		entitySystem.OnUpdate(deltaFrameCount, deltaTime);
	}

#if UNITY_EDITOR
	protected override void OnDrawGizmos()
    {
		entitySystem.OnDrawGizmos();
    }
#endif

}
