using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BattleSessionManager : IInputReceiver, IEntityCaptureReceiver
{
	private FrameInputMessage prevFrameInput = new FrameInputMessage();
	private FrameInputMessage currentFrameInput = new FrameInputMessage();

	private FrameEntityMessage[] currentFrameEntitiesSnapshot = null;
	private FrameEntityMessage currentFramePlayerEntitySnapshot = new FrameEntityMessage();

	public override void OnStartClient()
	{
		base.OnStartClient();

		NetworkClient.RegisterHandler<FrameSyncInputSnapShotMessage>(OnReceiveFrameSyncMessage);
		InputManager.Instance.RegisterInputReceiver(this);
		EntityManager.Instance.Register(this);
		IsServerSession = false;

		sessionCoroutine = StartCoroutine(DoClientRoutine());
	}

	private IEnumerator DoClientRoutine()
	{
		yield return null;
		// �� ������Ʈ�� ������ ���̶�� ��û�� �����ϴ�.

		yield return null;
		// �޾ƿ� ��� ������Ʈ ����Ʈ�� ObjectManager�� �ݿ��մϴ�.
	}

	public override void OnStopClient()
	{
		base.OnStopClient();

		NetworkClient.UnregisterHandler<FrameEntityMessage>();
		InputManager.Instance.UnregisterInputReceiver(this);
		EntityManager.Instance.UnRegister(this);

		IsServerSession = false;

		if (sessionCoroutine != null)
			StopCoroutine(sessionCoroutine);
	}

	public void Tick()
	{
		if (IsServerSession == false)
		{
			var currentFrameSnapShot = new FrameInputSnapShotMessage();
			currentFrameSnapShot.ownerGuid = 0;
			currentFrameSnapShot.tickCount = currentTickCount;
			currentFrameSnapShot.entityMessages = currentFrameEntitiesSnapshot;
			currentFrameSnapShot.playerEntityMessage = currentFramePlayerEntitySnapshot;
			currentFrameSnapShot.playerInputMessage = currentFrameInput;

			NetworkClient.Send(currentFrameSnapShot);
		}
	}

	private void OnReceiveFrameSyncMessage(FrameSyncInputSnapShotMessage message)
	{
		// ƽ ī��Ʈ�� ������ �ִٸ� ����Ѵ�.
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

	public void OnInput(FrameInputMessage input)
	{
		prevFrameInput = currentFrameInput;
		currentFrameInput = input;
	}

	public void OnCapture(FrameEntityMessage playerEntityMessage, FrameEntityMessage[] entitiyMessages)
	{
		currentFramePlayerEntitySnapshot = playerEntityMessage;
		currentFrameEntitiesSnapshot = entitiyMessages;
	}
}
