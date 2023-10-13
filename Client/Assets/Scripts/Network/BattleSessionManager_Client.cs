using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BattleSessionManager
{
	/// <summary>
	/// 틱 카운트 / 레이턴시
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
		// 클라이언트 선틱 처리
		OnTick?.Invoke(currentTickCount, clientLatencyTime);

		// 틱 카운트에 문제가 있다면 폐기
		if (currentTickCount > message.tickCount)
			return;

		// 위치는 기본적으로 알아서 동기화를 하고 있으므로, 무조건 재조정하지 않는다.
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

				// 충돌이 발생하여 위치값 보정이 필요한 Entity만 위치를 재조정한다.
				entity.Teleport(entityMessage.pos);

				// 스테이트도 당시 상황에 맞게 재조정
				entity.TryChangeState(userSnapShot.commandMessage);
			}
		}

		var sendMessage = MessageHelper.MakeSnapShot(GameManager.Instance.PlayerGuid, ++currentTickCount);

		NetworkClient.Send(sendMessage);
		clientLatencyTime = 0.0f;
	}
}
