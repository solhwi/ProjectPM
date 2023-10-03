using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OfflineBattleManager : Singleton<OfflineBattleManager>, IInputReceiver, IEntityCaptureReceiver
{
	private FrameInputSnapShotMessage snapShotMessage;

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
        snapShotMessage.entityMessages = entityMessages;
		snapShotMessage.playerEntityMessage = playerEntityMessage;
    }

	public override void OnPostUpdate(int deltaFrameCount, float deltaTime)
	{
		var frameSnapShot = new FrameSyncInputSnapShotMessage();
		frameSnapShot.snapshotMessages = new FrameInputSnapShotMessage[1];
		frameSnapShot.snapshotMessages[0] = snapShotMessage;
		frameSnapShot.snapshotMessages[0].entityMessages = FlushFrameSyncEntityMessage(snapShotMessage).ToArray();

        OnReceiveFrameSyncMessage(frameSnapShot);
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

            var simulatedNextState = entity.GetSimulatedNextState(snapShotMessage);
			if (simulatedNextState != (ENUM_ENTITY_STATE)entityMessage.entityState)
			{
				frameSyncMessage.animationMessage.normalizedTime = 0.0f;
				frameSyncMessage.animationMessage.keyFrame = 0;
            }

            yield return frameSyncMessage;
		}
	}

	private void OnReceiveFrameSyncMessage(FrameSyncInputSnapShotMessage message)
	{
		foreach (var snapShotMessage in message.snapshotMessages)
		{
			var entities = EntityManager.Instance.GetMyEntities(snapShotMessage.ownerGuid);
			if (entities == null)
				return;
			
			foreach(var entity in entities)
			{
                entity.TryChangeState(snapShotMessage);
            }
        }
	}
}
