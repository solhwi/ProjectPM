using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BattleSessionManager
{
	public override void OnStartClient()
	{
		base.OnStartClient();

		NetworkClient.RegisterHandler<FrameSyncSnapShotMessage>(OnReceiveFrameSyncMessage);
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
		IsServerSession = false;

		if (sessionCoroutine != null)
			StopCoroutine(sessionCoroutine);
	}

	public void Tick()
	{
		if (IsServerSession == false)
		{
			var currentFrameSnapShot = new FrameSnapShotMessage();
			currentFrameSnapShot.entityMessages = MakeFrameEntityMessages().ToArray();

			NetworkClient.Send(currentFrameSnapShot);
		}
	}

	private IEnumerable<FrameEntityMessage> MakeFrameEntityMessages()
	{
		foreach (var entity in EntityManager.Instance.GetMyEntities(0))
		{
			var entityMessage = new FrameEntityMessage();

			entityMessage.entityGuid = entity.Guid;
			entityMessage.isGrounded = entity.IsGrounded;
			entityMessage.myEntityOffset = entity.Offset;
			entityMessage.myEntityVelocity = entity.Velocity;
			entityMessage.myEntityHitBox = entity.HitBox;
			entityMessage.myEntityPos = entity.Position;
			entityMessage.entityCurrentState = (int)entity.CurrentState;

			yield return entityMessage;
		}
	}

	private void OnReceiveFrameSyncMessage(FrameSyncSnapShotMessage message)
	{
		// ƽ ī��Ʈ�� ������ �ִٸ� ����Ѵ�.
		foreach (var entityMessage in message.entityMessages)
		{
			var entity = EntityManager.Instance.GetEntityComponent(entityMessage.entityGuid);
			if (entity == null)
				return;

			entity.TryChangeState(entityMessage);
			entity.Teleport(entityMessage.myEntityPos);
		}
	}


}
