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
            // ���� ���忡�� ������ �����Ͻø� ����Ͽ� �ڷ�ƾ�� ����. 
            yield return new WaitForSeconds(latency);

            // �������� ��´�.
			int lastTickCount = inputMessageQueue.LastOrDefault().tickCount;

			var frameSnapShot = new FrameSyncInputSnapShotMessage();
			frameSnapShot.snapshotMessages = FlushFrameSyncEntityMessage(inputMessageQueue, lastTickCount).ToArray();
            frameSnapShot.tickCount = lastTickCount + 1;

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

            // ��ƼƼ���� �켱 ������ ��� ��Ȳ���� �̵� ��Ų ��
            foreach(var entityMessage in inputMessage.entityMessages)
            {
				var entity = EntityManager.Instance.GetEntityComponent(entityMessage.entityGuid);
				if (entity == null)
					continue;

				entity.Teleport(entityMessage.myEntityPos);
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

            // ������ �ȴٸ�, ���⼭ �ǵ����� ��
        }
    }

	private void OnReceiveFrameMessage(NetworkConnectionToClient connectionToClient, FrameInputSnapShotMessage message)
    {
        inputMessageQueue.Enqueue(message);
    }
}
