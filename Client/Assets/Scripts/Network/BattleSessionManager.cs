using kcp2k;
using Mirror;
using NPOI.HSSF.Record;
using NPOI.SS.Formula.Functions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;

public struct FrameSyncOutputMessage : NetworkMessage, IStateInfo
{
    public int lastTickCount;

    public int entityGuid;
    public int[] attackerEntities;
}

public struct FrameSyncInputMessage : NetworkMessage, IStateInfo
{
    public long sendTime;

    public Vector2 myEntityPos;
    public Vector2 myEntityHitBox;
    public Vector2 myEntityOffset;
    public Vector2 myEntityVelocity;

    public int tickCount;

    public int entityGuid;
    public int entityCurrentState;
}

public class BattleSessionManager : NetworkManager<BattleSessionManager>, ISessionComponent
{
    BattleSceneModuleParam battleParam = null;
    Queue<FrameSyncInputMessage> inputMessageQueue = new Queue<FrameSyncInputMessage>();

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

        NetworkClient.RegisterHandler<FrameSyncOutputMessage>(OnOutputFrameSyncMessage);
        IsServerSession = false;

        startSessionCoroutine = StartCoroutine(DoClientRoutine());
    }

    private IEnumerator DoClientRoutine()
    {
        yield return null;
        // �� ������Ʈ�� ������ ���̶�� ��û�� �����ϴ�.

        yield return null;
        // �޾ƿ� ��� ������Ʈ ����Ʈ�� ObjectManager�� �ݿ��մϴ�.
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        NetworkClient.UnregisterHandler<FrameSyncOutputMessage>();
        IsServerSession = false;

        if (startSessionCoroutine != null)
            StopCoroutine(startSessionCoroutine);
    }

    public void Tick()
    {
        if(IsServerSession)
        {
            // ������ �����Ͻ� üũ�� ���� �ڷ�ƾ���� �����մϴ�.
            // �����丵�� �ϴ��� ������
        }
        else
        {
            // Ŭ���̾�Ʈ���� ���� �ڽ��� ���� Ÿ���� �� ������ ��ǲ�� �־� ���� ���Դϴ�.
            // ��ǲ �Ŵ��� / ��ƼƼ �Ŵ��� ��� ������ ��� �ݿ��մϴ�.
            // ��ƼƼ���� ownerclientid, guid�� ������ ������, ownerclientid�� ������ ��ü�� ��� �����ϴ�.
            // �÷��̾� �ܿ� ���ݿ� ������Ʈ���� ���⿡ ���Ե˴ϴ�.

            // �ƹ����� �ѹ濡 ������ �� ���� �� �����ϴ�.
            NetworkClient.Send(new FrameSyncInputMessage());
        }       
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
        while(true)
        {
            // ���� ���忡�� ������ �����Ͻø� ����Ͽ� �ڷ�ƾ�� ����. 
            yield return new WaitForSeconds(1.0f);

            while (inputMessageQueue.TryDequeue(out var message))
            {
                var entities = EntityManager.Instance.GetOverlapEntities(message);
                if (entities == null)
                    continue;

                // tick count�� �־ �����ϴ�.
                // ���⿣ �ش� ƽ ī��Ʈ�� ���� entity ���� �浹 ó���� �����ϰ�,
                // �浹�� �� �ִ� ��ƼƼ�鸸 ��Ƽ� �����ϴ�.
                
                NetworkServer.SendToAll(new FrameSyncOutputMessage());
            }
        }

    }

    private void OnOutputFrameSyncMessage(FrameSyncOutputMessage message)
    {
        // ƽ ī��Ʈ�� ������ ī��Ʈ�� ���� �޽����� �� ���Ͽ�,
        // ƽ ī��Ʈ�� ������ �ִٸ� ����ϰ�, �ƴ� ��� state change�� �õ��մϴ�.
        // entity�� �ڽ��� �������� ƽ ī��Ʈ n������ �����ϰ� �����Ƿ�,
        // ���� ƽ ī��Ʈ�� ���������� hit state�� ���� �� �־��ٸ�,
        // ��ȭ��Ű�� �ڵ尡 �ۼ��Ǿ�� �մϴ�.
        var entity = EntityManager.Instance.GetEntityComponent(message.entityGuid);
        if (entity == null)
            return;

        entity.TryChangeState(message);
    }

    private void OnInputFrameSyncMessage(NetworkConnectionToClient connectionToClient, FrameSyncInputMessage message)
    {
        inputMessageQueue.Enqueue(message);
    }
}
