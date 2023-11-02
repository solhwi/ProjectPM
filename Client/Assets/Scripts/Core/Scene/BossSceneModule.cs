using Cysharp.Threading.Tasks;
using UnityEngine;

public class BossSceneModule : BattleSceneModule
{
	public class Param : SceneModuleParam
	{
		public readonly ENUM_MAP_TYPE mapType;
		public readonly ENUM_ENTITY_TYPE playerType;

		public Param(ENUM_MAP_TYPE mapType, ENUM_ENTITY_TYPE playerType)
		{
			this.mapType = mapType;
			this.playerType = playerType;
		}
	}


	public override void OnEnter(SceneModuleParam param)
	{
		base.OnEnter(param);

		mapSystem.Spawn(entitySystem.Player);
        entitySystem.ToPlayerControl();
		entitySystem.ToAIControl();

		foreach(var enemy in entitySystem.Enemies)
		{
			skillSystem.Register(enemy);
		}

        skillSystem.Register(entitySystem.Boss);
        skillSystem.Register(entitySystem.Player);
    }

	public override void OnExit()
	{
		base.OnExit();

        skillSystem.Unregister(entitySystem.Player);
    }

    public async override UniTask OnPrepareEnterRoutine(SceneModuleParam param)
	{
		var _param = param as Param;
		if (_param == null)
		{
			Debug.LogError("���� �� ��⿡ �����ϱ� ���� �Ķ���Ͱ� �̻��մϴ�.");
			return;	
		}

        await mapSystem.CreateMap(_param.mapType); // �� ����
        await entitySystem.CreatePlayer(_param.playerType); // �÷��̾� ����
		await entitySystem.CreateEnemy(ENUM_ENTITY_TYPE.PencilMan);
    }

	public override void OnFixedUpdate(int tickCount, float latencyTime)
	{
		physicsSystem.OnUpdate(tickCount, latencyTime);
	}

	public override void OnPrevUpdate(int deltaFrameCount, float deltaTime)
	{
		entitySystem.OnUpdate(deltaFrameCount, deltaTime);
	}

	public override void OnUpdate(int deltaFrameCount, float deltaTime)
	{
		base.OnUpdate(deltaFrameCount, deltaTime);
		skillSystem.OnUpdate(deltaFrameCount, deltaTime);
	}

	protected override void OnDrawGizmos()
    {
		entitySystem.OnDrawGizmos();
    }
}
