using MBTEditor;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BattleSessionManager : IInputReceiver, IEntityCaptureReceiver
{
	private FrameInputSnapShotMessage currentSnapShot = new FrameInputSnapShotMessage();

    public override void OnStartClient()
	{
		base.OnStartClient();

		NetworkClient.RegisterHandler<FrameSyncInputSnapShotMessage>(OnReceiveFrameSyncMessage);

        latency = new WaitForSecondsRealtime(NetworkClient.sendInterval);
        sessionCoroutine = StartCoroutine(DoClientRoutine());
	}

	private IEnumerator DoClientRoutine()
	{
		yield return null;
		// 내 오브젝트를 생성할 것이라는 요청을 보냅니다.

		yield return null;
        // 받아온 모든 오브젝트 리스트를 ObjectManager에 반영합니다.

        while (true)
        {
            yield return latency;

            currentSnapShot.ownerGuid = EntityManager.Instance.PlayerGuid;
            currentSnapShot.tickCount = currentTickCount;

            NetworkClient.Send(currentSnapShot);
        }
    }

	public override void OnStopClient()
	{
		base.OnStopClient();

		NetworkClient.UnregisterHandler<FrameEntityMessage>();
        latency = null;

        if (sessionCoroutine != null)
			StopCoroutine(sessionCoroutine);
	}

	private void OnReceiveFrameSyncMessage(FrameSyncInputSnapShotMessage message)
	{
		// 틱 카운트에 문제가 있다면 폐기한다.
		if (currentTickCount > message.tickCount)
			return;

		foreach (var snapshotMessage in message.snapshotMessages)
		{
			foreach(var entity in snapshotMessage.entityMessages)
			{
				var entityComponent = EntityManager.Instance.GetEntityComponent(entity.entityGuid);
				if (entityComponent == null)
					continue;

                entityComponent.Teleport(entity.myEntityPos);
                entityComponent.TryChangeState(snapshotMessage);
            }
		}

		currentTickCount = message.tickCount + 1;
	}

    public void OnInput(FrameInputMessage resultInput)
    {
        currentSnapShot.playerInputMessage = resultInput;
    }

    public void OnCapture(FrameEntityMessage playerEntityMessage, FrameEntityMessage[] entitiyMessages)
    {
        currentSnapShot.playerEntityMessage = playerEntityMessage;
        currentSnapShot.entityMessages = entitiyMessages;
    }
}
