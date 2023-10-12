using kcp2k;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneModule : NetworkSceneModule
{
    private BattleSessionManager battleSessionManager = null;

#if UNITY_EDITOR
	protected override NetworkManager CreateNetworkManager()
	{
        var networkManager = FindObjectOfType<BattleSessionManager>();
        if (networkManager == null)
        {
            var g = new GameObject("BattleSessionManager");
            g.transform.SetParent(transform);

            networkManager = g.AddComponent<BattleSessionManager>();
        }
        return networkManager;
    }

    protected override Transport CreateTransport()
    {
        var transport = FindObjectOfType<KcpTransport>();
        if (transport == null)
        {
            var g = new GameObject("KcpTransport");
            g.transform.SetParent(transform);

            transport = g.AddComponent<KcpTransport>();
        }

        return transport;
    }
#endif

	public override void OnEnter(SceneModuleParam param)
	{
		base.OnEnter(param);

		battleSessionManager =  currentSession as BattleSessionManager;
        if (battleSessionManager == null)
            return;

        battleSessionManager.OnTick += OnTick;
	}

	public override void OnExit()
	{
		battleSessionManager.OnTick -= OnTick;

		base.OnExit();
	}

	private void OnTick(int tickCount, float latencyTime)
    {
        PhysicsManager.Instance.OnFixedUpdate(tickCount, latencyTime);
    }

	public override void OnPrevUpdate(int deltaFrameCount, float deltaTime)
	{
		InputManager.Instance.OnUpdate(deltaFrameCount, deltaTime);
	}

	public override void OnUpdate(int deltaFrameCount, float deltaTime)
	{
        EntityManager.Instance.OnUpdate(deltaFrameCount, deltaTime);
	}
}