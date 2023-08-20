using kcp2k;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSessionManager : NetworkRoomManager, ISessionComponent
{
    public void Connect()
    {
        StartClient();
    }

    public void Disconnect()
    {
        StopClient();
    }
}
