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
        // 내 오브젝트를 생성할 것이라는 요청을 보냅니다.

        yield return null;
        // 받아온 모든 오브젝트 리스트를 ObjectManager에 반영합니다.
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
            // 서버는 레이턴시 체크를 위해 코루틴으로 대응합니다.
            // 리팩토링을 하던가 말던가
        }
        else
        {
            // 클라이언트들은 각자 자신의 로컬 타임을 매 프레임 인풋에 넣어 보낼 것입니다.
            // 인풋 매니저 / 앤티티 매니저 등에서 정보를 모아 반영합니다.
            // 앤티티들은 ownerclientid, guid를 가지고 있으며, ownerclientid가 본인인 객체만 모아 보냅니다.
            // 플레이어 외에 공격용 오브젝트들이 여기에 포함됩니다.

            // 아무래도 한방에 보내는 게 나을 거 같습니다.
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
            // 서버 입장에서 적절한 레이턴시를 계산하여 코루틴을 돈다. 
            yield return new WaitForSeconds(1.0f);

            while (inputMessageQueue.TryDequeue(out var message))
            {
                var entities = EntityManager.Instance.GetOverlapEntities(message);
                if (entities == null)
                    continue;

                // tick count를 넣어서 보냅니다.
                // 여기엔 해당 틱 카운트에 쌓인 entity 들의 충돌 처리를 감지하고,
                // 충돌된 게 있는 앤티티들만 모아서 보냅니다.
                
                NetworkServer.SendToAll(new FrameSyncOutputMessage());
            }
        }

    }

    private void OnOutputFrameSyncMessage(FrameSyncOutputMessage message)
    {
        // 틱 카운트가 본인의 카운트와 같은 메시지인 지 비교하여,
        // 틱 카운트에 문제가 있다면 폐기하고, 아닌 경우 state change를 시도합니다.
        // entity는 자신의 스냅샷을 틱 카운트 n개까지 저장하고 있으므로,
        // 같은 틱 카운트의 스냅샷에서 hit state로 변할 수 있었다면,
        // 변화시키는 코드가 작성되어야 합니다.
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
