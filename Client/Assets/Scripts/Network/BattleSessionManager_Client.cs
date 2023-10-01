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
		// 내 오브젝트를 생성할 것이라는 요청을 보냅니다.

		yield return null;
		// 받아온 모든 오브젝트 리스트를 ObjectManager에 반영합니다.
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
		// 틱 카운트에 문제가 있다면 폐기한다.
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
