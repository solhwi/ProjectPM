using kcp2k;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSceneModule : NetworkSceneModule
{
	public class Param : SceneModuleParam
	{
		public readonly bool isOwner = false;

		public Param(bool isOwner)
		{
			this.isOwner = isOwner;
		}
	}

#if UNITY_EDITOR
	protected override NetworkManager CreateNetworkManager()
	{
        var networkManager = FindObjectOfType<MatchSessionManager>();
        if (networkManager == null)
        {
            var g = new GameObject("MatchSessionManager");
            g.transform.SetParent(transform);

            networkManager = g.AddComponent<MatchSessionManager>();
        }
        return networkManager;
    }

    protected override Transport CreateTransport()
    {
        var transport = FindObjectOfType<TelepathyTransport>();
        if (transport == null)
        {
            var g = new GameObject("TelepathyTransport");
            g.transform.SetParent(transform);

            transport = g.AddComponent<TelepathyTransport>();
        }

        return transport;
    }
#endif

}
