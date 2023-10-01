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
    private float latency = 1.0f;

   
    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<FrameSnapShotMessage>(OnReceiveFrameMessage, false);
        IsServerSession = true;

        sessionCoroutine = StartCoroutine(DoServerRoutine());
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        NetworkServer.UnregisterHandler<FrameSnapShotMessage>();
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

			var frameSnapShot = new FrameSyncSnapShotMessage();
			frameSnapShot.entityMessages = FlushFrameSyncEntityMessage(inputMessageQueue, lastTickCount).ToArray();

            // �������� �߼��Ѵ�.
			NetworkServer.SendToAll(frameSnapShot);
		}
	}

    private IEnumerable<FrameEntityMessage> FlushFrameSyncEntityMessage(Queue<FrameSnapShotMessage> inputQueue, int tickIndex)
    {
		while (inputQueue.TryDequeue(out var message))
		{
            if (message.tickCount != tickIndex)
                continue;

            // �켱 ������ ��� ��Ȳ���� �̵� ��Ų ��
            foreach(var entityMessage in message.entityMessages)
            {
				var entity = EntityManager.Instance.GetEntityComponent(entityMessage.entityGuid);
				if (entity == null)
					continue;

				entity.Teleport(entityMessage.myEntityPos);
			}

            // �浹 üũ�Ѵ�.
			foreach (var entityMessage in message.entityMessages)
			{
				var frameSyncMessage = new FrameEntityMessage();
				frameSyncMessage.entityGuid = entityMessage.entityGuid;
				frameSyncMessage.attackerEntities = EntityManager.Instance.GetOverlapEntitiyGuids(entityMessage).ToArray();
				frameSyncMessage.entityCurrentState = entityMessage.entityCurrentState;
				yield return frameSyncMessage;
			}

            // ������ �ȴٸ�, ���⼭ �ǵ����� ��
		}
	}

	private void OnReceiveFrameMessage(NetworkConnectionToClient connectionToClient, FrameSnapShotMessage message)
    {
        inputMessageQueue.Enqueue(message);
    }
}
