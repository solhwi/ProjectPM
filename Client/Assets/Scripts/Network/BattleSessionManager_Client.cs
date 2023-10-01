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

		NetworkClient.RegisterHandler<FrameSyncSnapShotMessage>(OnReceiveFrameSyncMessage);
		InputManager.Instance.RegisterInputReceiver(this);
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

	private void OnReceiveFrameSyncMessage(FrameSyncSnapShotMessage message)
	{
		// ƽ ī��Ʈ�� ������ �ִٸ� ����Ѵ�.
		if (currentTickCount > message.tickCount)
			return;

		foreach (var entityMessage in message.entityMessages)
		{
			var entity = EntityManager.Instance.GetEntityComponent(entityMessage.entityGuid);
			if (entity == null)
				return;

			entity.Teleport(entityMessage.myEntityPos);
			entity.TryChangeState(entityMessage);
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
