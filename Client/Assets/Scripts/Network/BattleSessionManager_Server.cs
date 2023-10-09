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
   
    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<FrameInputSnapShotMessage>(OnReceiveFrameMessage, false);

        latency = new WaitForSecondsRealtime(NetworkServer.tickInterval);
        sessionCoroutine = StartCoroutine(DoServerRoutine());
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        NetworkServer.UnregisterHandler<FrameInputSnapShotMessage>();
        latency = null;

        if (sessionCoroutine != null)
            StopCoroutine(sessionCoroutine);
    }

    private IEnumerator DoServerRoutine()
    {
        while(true)
        {
            // ���� ���忡�� ������ �����Ͻø� ����Ͽ� �ڷ�ƾ�� ����. 
            yield return latency;

            // �������� ��´�.
            int lastTickCount = inputMessageQueue.LastOrDefault().tickCount;

			var frameSnapShot = new FrameSyncInputSnapShotMessage();
			frameSnapShot.snapshotMessages = FlushFrameSyncEntityMessage(inputMessageQueue, lastTickCount).ToArray();
            frameSnapShot.tickCount = lastTickCount;

			// �������� �߼��Ѵ�.
			NetworkServer.SendToAll(frameSnapShot);
		}
	}

    private IEnumerable<FrameInputSnapShotMessage> FlushFrameSyncEntityMessage(Queue<FrameInputSnapShotMessage> inputQueue, int tickIndex)
    {
		while (inputQueue.TryDequeue(out var inputMessage))
		{
            if (inputMessage.tickCount != tickIndex)
                continue;

            var entityPrevPosList = new List<KeyValuePair<EntityComponent, Vector2>>();
            
            // ��ƼƼ���� �켱 ������ ��� ��Ȳ���� �̵� ��Ų ��
            foreach(var entityMessage in inputMessage.entityMessages)
            {
				var entity = EntityManager.Instance.GetEntityComponent(entityMessage.entityGuid);
				if (entity == null)
					continue;

                entityPrevPosList.Add(new KeyValuePair<EntityComponent, Vector2>(entity, entity.Position));
                entity.SetPosition(entityMessage.myEntityPos);
			}

            // �浹 ������ �־� ������.
			for (int i = 0; i < inputMessage.entityMessages.Length; i++)
			{
                var entity = EntityManager.Instance.GetEntityComponent(inputMessage.entityMessages[i].entityGuid);
				if (entity == null)
					continue;

                inputMessage.entityMessages[i].attackerEntities = EntityManager.Instance.GetOverlapEntitiyGuids(inputMessage.entityMessages[i]).ToArray();
			}

            yield return inputMessage;

            // ���� ��ġ��� �ǵ���
            foreach(var entityPair in entityPrevPosList)
            {
                var entity = entityPair.Key;
                var prevPos = entityPair.Value;

                entity.SetPosition(prevPos);
            }
        }
    }

	private void OnReceiveFrameMessage(NetworkConnectionToClient connectionToClient, FrameInputSnapShotMessage message)
    {
        inputMessageQueue.Enqueue(message);
    }
}
