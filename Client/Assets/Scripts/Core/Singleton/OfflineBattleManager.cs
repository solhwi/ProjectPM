using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OfflineBattleManager : Singleton<OfflineBattleManager>, IEntityCaptureReceiver
{
	private FrameEntityMessage[] currentFrameEntitiesSnapshot = null;

	protected override void OnAwakeInstance()
	{
		EntityManager.Instance.Register(this);
	}

	protected override void OnReleaseInstance()
	{
		EntityManager.Instance.UnRegister(this);
	}

	public void OnCapture(FrameEntityMessage playerEntityMessage, FrameEntityMessage[] entityMessages)
	{
		currentFrameEntitiesSnapshot = entityMessages;
	}

	public override void OnLateUpdate(int deltaFrameCount, float deltaTime)
	{
		if (currentFrameEntitiesSnapshot == null)
			return;

		var frameSnapShot = new FrameSyncSnapShotMessage();
		frameSnapShot.entityMessages = FlushFrameSyncEntityMessage(currentFrameEntitiesSnapshot).ToArray();

		OnReceiveFrameSyncMessage(frameSnapShot);
	}

	private IEnumerable<FrameEntityMessage> FlushFrameSyncEntityMessage(IEnumerable<FrameEntityMessage> entityMessages)
	{
		// 충돌 체크한다.
		foreach (var entityMessage in entityMessages)
		{
			var entity = EntityManager.Instance.GetEntityComponent(entityMessage.entityGuid);
			if (entity == null)
				continue;

			var frameSyncMessage = new FrameEntityMessage();
			frameSyncMessage.entityGuid = entityMessage.entityGuid;
			frameSyncMessage.attackerEntities = EntityManager.Instance.GetOverlapEntitiyGuids(entityMessage).ToArray();
			yield return frameSyncMessage;
		}
	}

	private void OnReceiveFrameSyncMessage(FrameSyncSnapShotMessage message)
	{
		foreach (var entityMessage in message.entityMessages)
		{
			var entity = EntityManager.Instance.GetEntityComponent(entityMessage.entityGuid);
			if (entity == null)
				return;

			entity.TryChangeState(entityMessage);
		}
	}
}
