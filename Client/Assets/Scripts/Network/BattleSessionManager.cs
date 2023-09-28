using kcp2k;
using Mirror;
using NPOI.SS.Formula.Functions;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;

public struct FrameSyncOutputMessage : NetworkMessage
{

}

public class BattleSessionManager : NetworkManager<BattleSessionManager>, ISessionComponent, IInputReceiver
{
    BattleSceneModuleParam battleParam = null;
    Queue<KeyValuePair<int, FrameSyncInputMessage>> inputMessageQueue = new Queue<KeyValuePair<int, FrameSyncInputMessage>>();

    Coroutine startSessionCoroutine = null;

    private bool IsServerSession = false;

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

    public override void OnStartClient()
    {
        base.OnStartClient();

        InputManager.Instance.RegisterInputReceiver(this);
        IsServerSession = false;

        startSessionCoroutine = StartCoroutine(DoClientRoutine());
    }

    private IEnumerator DoClientRoutine()
    {
        yield return null;
        // 내 오브젝트를 생성할 것이라는 요청을 보냅니다.

        yield return null;
        // 받아온 모든 오브젝트 리스트를 ObjectManager에 반영합니다.

        yield return null;
        // 인풋 매니저를 가동하고,
        // 클라이언트들은 각자 자신의 로컬 타임을 인풋에 넣어 보낼 것입니다.

        yield return null;
        // 서버의 아웃풋 데이터를 받아 오브젝트들의 상태를 반영합니다.
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        InputManager.Instance.UnregisterInputReceiver(this);
        IsServerSession = false;

        if(startSessionCoroutine != null)
            StopCoroutine(startSessionCoroutine);

    }

    public void Tick()
    {
       
    }

    // 인풋을 바로바로 보낸다.
    public void OnInput(FrameSyncInputMessage resultInput)
    {
        NetworkClient.Send(resultInput);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<FrameSyncInputMessage>(OnInputFrameSyncMessage, false);
        IsServerSession = true;

        startSessionCoroutine = StartCoroutine(DoServerRoutine());
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        NetworkServer.UnregisterHandler<FrameSyncInputMessage>();
        IsServerSession = false;

        if (startSessionCoroutine != null)
            StopCoroutine(startSessionCoroutine);
    }

    private IEnumerator DoServerRoutine()
    {
        yield return new WaitForSeconds(1.0f);

        // 현재 오브젝트들의 상태를 가져온다.

        // 서버 입장에서 레이턴시를 계산하여 적법한 친구들을 사용하여 아웃풋 데이터를 만든다.

        NetworkServer.SendToAll(new FrameSyncOutputMessage());
        // BroadCast를 수행한다.
    }

    private void OnInputFrameSyncMessage(NetworkConnectionToClient connectionToClient, FrameSyncInputMessage message)
    {
        inputMessageQueue.Enqueue(new KeyValuePair<int, FrameSyncInputMessage>(connectionToClient.connectionId, message));
    }
}
