using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BattleSessionManager
{
	/// <summary>
	/// ƽ ī��Ʈ / �����Ͻ�
	/// </summary>
	public event Action<int, float> OnTick = null;

    public override void OnStartClient()
	{
		base.OnStartClient();

		NetworkClient.RegisterHandler<FrameSyncSnapShotMessage>(OnReceiveFrameSyncMessage);
	}

	public override void OnStopClient()
	{
		base.OnStopClient();

		NetworkClient.UnregisterHandler<FrameSyncSnapShotMessage>();
	}

	public override void Update()
	{
		base.Update();
		clientLatencyTime += Time.deltaTime;
	}

	private void OnReceiveFrameSyncMessage(FrameSyncSnapShotMessage message)
	{
		// Ŭ���̾�Ʈ ��ƽ ó��
		OnTick?.Invoke(currentTickCount, clientLatencyTime);

		// ƽ ī��Ʈ�� ������ �ִٸ� ���
		if (currentTickCount > message.tickCount)
			return;

		// ��ġ�� �⺻������ �˾Ƽ� ����ȭ�� �ϰ� �����Ƿ�, ������ ���������� �ʴ´�.
		foreach (var userSnapShot in message.snapshotMessages)
		{
			foreach(var entityMessage in userSnapShot.entityMessages)
			{
				if (entityMessage.overlappedEntities == null)
					continue;

				if (entityMessage.overlappedEntities.Length <= 0)
					continue;

				var entityGuid = entityMessage.entityGuid;

				var entity = EntityManager.Instance.GetEntityComponent(entityGuid);
				if (entity == null)
					continue;

				// �浹�� �߻��Ͽ� ��ġ�� ������ �ʿ��� Entity�� ��ġ�� �������Ѵ�.
				entity.Teleport(entityMessage.pos);

				// ������Ʈ�� ��� ��Ȳ�� �°� ������
				entity.TryChangeState(userSnapShot.commandMessage);
			}
		}

		var sendMessage = MessageHelper.MakeSnapShot(GameManager.Instance.PlayerGuid, ++currentTickCount);

		NetworkClient.Send(sendMessage);
		clientLatencyTime = 0.0f;
	}
}
