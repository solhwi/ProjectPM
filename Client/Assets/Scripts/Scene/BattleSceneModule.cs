using kcp2k;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneModule : NetworkSceneModule
{
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
}