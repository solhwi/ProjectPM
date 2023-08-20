using kcp2k;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSessionManager : NetworkRoomManager<BattleSessionManager>, ISessionComponent
{
    BattleSceneModuleParam battleParam = null;

    public void Connect(SceneModuleParam param)
    {
        if (param is BattleSceneModuleParam battleParam)
        {
            this.battleParam = battleParam;

            if (battleParam.isOwner)
            {
                StartHost();
            }
            else
            {
                StartClient();
            }
        }
    }

    public void Disconnect()
    {
        if (battleParam.isOwner)
        {
            StopHost();
        }
        else
        {
            StopClient();
        }
    }
}
