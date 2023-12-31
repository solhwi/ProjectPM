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
    Queue<FrameSnapShotMessage> inputMessageQueue = new Queue<FrameSnapShotMessage>();
   
    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<FrameSnapShotMessage>(OnReceiveFrameMessage, false);

		serverTickFrequency = new WaitForSecondsRealtime(NetworkServer.tickInterval);
        sessionCoroutine = StartCoroutine(DoServerRoutine());
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        NetworkServer.UnregisterHandler<FrameSnapShotMessage>();
		serverTickFrequency = null;

        if (sessionCoroutine != null)
            StopCoroutine(sessionCoroutine);
    }

    private IEnumerator DoServerRoutine()
    {
        while(true)
        {
            // 서버 입장에서 적절한 레이턴시를 선 계산하여 코루틴을 돈다. 
            yield return serverTickFrequency;

            // 적절한 tick count 사용 고민
            int lastTickCount = inputMessageQueue.LastOrDefault().tickCount;

			var frameSnapShot = new FrameSyncSnapShotMessage();
			frameSnapShot.snapshotMessages = FlushFrameSyncEntityMessage(inputMessageQueue, lastTickCount).ToArray();
            frameSnapShot.tickCount = lastTickCount;

			// 스냅샷을 발송한다.
			NetworkServer.SendToAll(frameSnapShot);
		}
	}

    // 서버에서 충돌을 처리하여 넣어 보냄
    private IEnumerable<FrameSnapShotMessage> FlushFrameSyncEntityMessage(Queue<FrameSnapShotMessage> inputQueue, int tickIndex)
    {
		while (inputQueue.TryDequeue(out var inputMessage))
		{
            if (inputMessage.tickCount != tickIndex)
                continue;

            var entityPrevPosList = new List<KeyValuePair<IEntity, Vector2>>();
            
            // 앤티티들을 우선 프레임 당시 상황으로 이동 시킨 후
            foreach(var entityMessage in inputMessage.entityMessages)
            {
				var entity = entitySystem.GetEntity(entityMessage.entityGuid);
				if (entity == null)
					continue;
                 
                entityPrevPosList.Add(new KeyValuePair<IEntity, Vector2>(entity, entity.Position));
                entity.SetPosition(entityMessage.pos);
			}

            // 충돌 정보를 넣어 보낸다.
			foreach(var entityMessage in inputMessage.entityMessages)
			{
		        entityMessage.AddAttackerEntities(entitySystem);
			}

            yield return inputMessage;

            // 원래 위치대로 되돌림
            foreach(var entityPair in entityPrevPosList)
            {
                var entity = entityPair.Key;
                var prevPos = entityPair.Value;

                entity.SetPosition(prevPos);
            }
        }
    }

	private void OnReceiveFrameMessage(NetworkConnectionToClient connectionToClient, FrameSnapShotMessage message)
    {
        inputMessageQueue.Enqueue(message);
    }
}
