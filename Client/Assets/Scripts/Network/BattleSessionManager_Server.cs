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

public partial class BattleSessionManager
{
    Queue<FrameInputSnapShotMessage> inputMessageQueue = new Queue<FrameInputSnapShotMessage>();
    private float latency = 1.0f;

   
    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<FrameInputSnapShotMessage>(OnReceiveFrameMessage, false);
		IsServerSession = true;

        sessionCoroutine = StartCoroutine(DoServerRoutine());
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        NetworkServer.UnregisterHandler<FrameInputSnapShotMessage>();
		IsServerSession = false;

        if (sessionCoroutine != null)
            StopCoroutine(sessionCoroutine);
    }

    private IEnumerator DoServerRoutine()
    {
        while(true)
        {
            // 서버 입장에서 적절한 레이턴시를 계산하여 코루틴을 돈다. 
            yield return new WaitForSeconds(latency);

            // 스냅샷을 찍는다.
			int lastTickCount = inputMessageQueue.LastOrDefault().tickCount;

			var frameSnapShot = new FrameSyncSnapShotMessage();
			frameSnapShot.entityMessages = FlushFrameSyncEntityMessage(inputMessageQueue, lastTickCount).ToArray();
            frameSnapShot.tickCount = lastTickCount + 1;

			// 스냅샷을 발송한다.
			NetworkServer.SendToAll(frameSnapShot);
		}
	}

    private IEnumerable<FrameEntityMessage> FlushFrameSyncEntityMessage(Queue<FrameInputSnapShotMessage> inputQueue, int tickIndex)
    {
		while (inputQueue.TryDequeue(out var inputMessage))
		{
            if (inputMessage.tickCount != tickIndex)
                continue;

            // 우선 프레임 당시 상황으로 이동 시킨 후
            foreach(var entityMessage in inputMessage.entityMessages)
            {
				var entity = EntityManager.Instance.GetEntityComponent(entityMessage.entityGuid);
				if (entity == null)
					continue;

				entity.Teleport(entityMessage.myEntityPos);
			}

            // 충돌 체크한다.
			foreach (var entityMessage in inputMessage.entityMessages)
			{
				var entity = EntityManager.Instance.GetEntityComponent(entityMessage.entityGuid);
				if (entity == null)
					continue;

				var frameSyncMessage = new FrameEntityMessage();
				frameSyncMessage.entityGuid = entityMessage.entityGuid;
				frameSyncMessage.attackerEntities = EntityManager.Instance.GetOverlapEntitiyGuids(entityMessage).ToArray();
                frameSyncMessage.entityState = (int)entity.GetSimulatedNextState(inputMessage.playerInputMessage);
				yield return frameSyncMessage;
			}

            // 문제가 된다면, 여기서 되돌려도 됨
		}
	}

	private void OnReceiveFrameMessage(NetworkConnectionToClient connectionToClient, FrameInputSnapShotMessage message)
    {
        inputMessageQueue.Enqueue(message);
    }
}
