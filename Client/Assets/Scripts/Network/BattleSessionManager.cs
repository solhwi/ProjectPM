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
        // �� ������Ʈ�� ������ ���̶�� ��û�� �����ϴ�.

        yield return null;
        // �޾ƿ� ��� ������Ʈ ����Ʈ�� ObjectManager�� �ݿ��մϴ�.

        yield return null;
        // ��ǲ �Ŵ����� �����ϰ�,
        // Ŭ���̾�Ʈ���� ���� �ڽ��� ���� Ÿ���� ��ǲ�� �־� ���� ���Դϴ�.

        yield return null;
        // ������ �ƿ�ǲ �����͸� �޾� ������Ʈ���� ���¸� �ݿ��մϴ�.
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

    // ��ǲ�� �ٷιٷ� ������.
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

        // ���� ������Ʈ���� ���¸� �����´�.

        // ���� ���忡�� �����Ͻø� ����Ͽ� ������ ģ������ ����Ͽ� �ƿ�ǲ �����͸� �����.

        NetworkServer.SendToAll(new FrameSyncOutputMessage());
        // BroadCast�� �����Ѵ�.
    }

    private void OnInputFrameSyncMessage(NetworkConnectionToClient connectionToClient, FrameSyncInputMessage message)
    {
        inputMessageQueue.Enqueue(new KeyValuePair<int, FrameSyncInputMessage>(connectionToClient.connectionId, message));
    }
}
