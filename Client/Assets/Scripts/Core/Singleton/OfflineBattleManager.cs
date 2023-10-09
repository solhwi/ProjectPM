using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OfflineBattleManager : Singleton<OfflineBattleManager>, IInputReceiver, IEntityCaptureReceiver
{
	private FrameInputSnapShotMessage snapShotMessage = new FrameInputSnapShotMessage();

	protected override void OnAwakeInstance()
	{
		InputManager.Instance.RegisterInputReceiver(this);
		EntityManager.Instance.Register(this);
	}

	protected override void OnReleaseInstance()
	{
		EntityManager.Instance.UnRegister(this);
        InputManager.Instance.UnregisterInputReceiver(this);
    }

    public void OnInput(FrameInputMessage message)
	{
        snapShotMessage.playerInputMessage = message;
    }

	public void OnCapture(FrameEntityMessage playerEntityMessage, FrameEntityMessage[] entityMessages)
	{
        snapShotMessage.playerEntityMessage = playerEntityMessage;
        snapShotMessage.entityMessages = entityMessages;
    }

	public override void OnPostUpdate(int deltaFrameCount, float deltaTime)
	{
		snapShotMessage.ownerGuid = EntityManager.Instance.PlayerGuid;
		snapShotMessage.tickCount = deltaFrameCount;
		snapShotMessage.entityMessages = FlushFrameSyncEntityMessage(snapShotMessage).ToArray();

        OnReceiveFrameSyncMessage(snapShotMessage);
	}

	private IEnumerable<FrameEntityMessage> FlushFrameSyncEntityMessage(FrameInputSnapShotMessage snapShotMessage)
	{
		if (snapShotMessage.entityMessages == null)
			yield break;

		// 충돌 체크한다.
		foreach (var entityMessage in snapShotMessage.entityMessages)
		{
			var entity = EntityManager.Instance.GetEntityComponent(entityMessage.entityGuid);
			if (entity == null)
				continue;

			var frameSyncMessage = new FrameEntityMessage();
			frameSyncMessage.entityGuid = entityMessage.entityGuid;
			frameSyncMessage.entityState = entityMessage.entityState;

			frameSyncMessage.myEntityOffset = entityMessage.myEntityOffset;
			frameSyncMessage.myEntityHitBox = entityMessage.myEntityHitBox;
			frameSyncMessage.myEntityPos = entityMessage.myEntityPos;

			frameSyncMessage.attackerEntities = EntityManager.Instance.GetOverlapEntitiyGuids(entityMessage).ToArray();
            frameSyncMessage.animationMessage = entityMessage.animationMessage;

            yield return frameSyncMessage;
		}
	}

	private void OnReceiveFrameSyncMessage(FrameInputSnapShotMessage message)
	{
        var entities = EntityManager.Instance.GetAllEntities();
        if (entities == null)
            return;

        foreach (var entity in entities)
        {
            entity.TryChangeState(message);
        }
    }
}
