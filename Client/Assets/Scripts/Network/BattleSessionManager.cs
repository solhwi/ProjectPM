using kcp2k;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSessionManager : NetworkRoomManager, ISessionComponent
{
    public override void Initialize()
    {
        gameObject.AddComponent<KcpTransport>();
        base.Initialize();
    }

    public void Connect()
    {
        StartClient();
    }

    public void Disconnect()
    {
        StopClient();
    }
}
