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
            // ���� ���忡�� ������ �����Ͻø� �� ����Ͽ� �ڷ�ƾ�� ����. 
            yield return serverTickFrequency;

            // ������ tick count ��� ���
            int lastTickCount = inputMessageQueue.LastOrDefault().tickCount;

			var frameSnapShot = new FrameSyncSnapShotMessage();
			frameSnapShot.snapshotMessages = FlushFrameSyncEntityMessage(inputMessageQueue, lastTickCount).ToArray();
            frameSnapShot.tickCount = lastTickCount;

			// �������� �߼��Ѵ�.
			NetworkServer.SendToAll(frameSnapShot);
		}
	}

    // �������� �浹�� ó���Ͽ� �־� ����
    private IEnumerable<FrameSnapShotMessage> FlushFrameSyncEntityMessage(Queue<FrameSnapShotMessage> inputQueue, int tickIndex)
    {
		while (inputQueue.TryDequeue(out var inputMessage))
		{
            if (inputMessage.tickCount != tickIndex)
                continue;

            var entityPrevPosList = new List<KeyValuePair<EntityComponent, Vector2>>();
            
            // ��ƼƼ���� �켱 ������ ��� ��Ȳ���� �̵� ��Ų ��
            foreach(var entityMessage in inputMessage.entityMessages)
            {
				var entity = EntitySystem.Instance.GetEntityComponent(entityMessage.entityGuid);
				if (entity == null)
					continue;

                entityPrevPosList.Add(new KeyValuePair<EntityComponent, Vector2>(entity, entity.Position));
                entity.Teleport(entityMessage.pos);
			}

            // �浹 ������ �־� ������.
			for (int i = 0; i < inputMessage.entityMessages.Length; i++)
			{
                var entity = EntitySystem.Instance.GetEntityComponent(inputMessage.entityMessages[i].entityGuid);
				if (entity == null)
					continue;

                inputMessage.entityMessages[i].overlappedEntities = PhysicsSystem.Collision.GetOverlapEntitiyGuids(inputMessage.entityMessages[i]).ToArray();
			}

            yield return inputMessage;

            // ���� ��ġ��� �ǵ���
            foreach(var entityPair in entityPrevPosList)
            {
                var entity = entityPair.Key;
                var prevPos = entityPair.Value;

                entity.Teleport(prevPos);
            }
        }
    }

	private void OnReceiveFrameMessage(NetworkConnectionToClient connectionToClient, FrameSnapShotMessage message)
    {
        inputMessageQueue.Enqueue(message);
    }
}
