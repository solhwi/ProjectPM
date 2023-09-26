using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이 정보들로 위치, 공격, 스킬을 시전한다.
public struct NetworkFrameSyncData
{

}

// 이후 위치가 이동하게 되면, 충돌이 발생하고, 충돌이 발생했다면 Hit State가 된다.
// 충돌까지 같이 처리해줘야 할까?

public class NetworkSyncMeditator : NetworkBehaviour
{
	Queue<NetworkFrameSyncData> frameSyncQueue = new Queue<NetworkFrameSyncData>();

	public override void OnStartServer()
	{
		base.OnStartServer();
	}
	public override void OnStopServer()
	{
		base.OnStopServer();
	}

	public override void OnStartAuthority()
	{
		base.OnStartAuthority();
	}
	public override void OnStopAuthority()
	{
		base.OnStopAuthority();
	}

	public override void OnStartClient()
	{
		base.OnStartClient();
	}
	public override void OnStopClient()
	{
		base.OnStopClient();
	}

	public override void OnSerialize(NetworkWriter writer, bool initialState)
	{
		base.OnSerialize(writer, initialState);
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
		base.OnDeserialize(reader, initialState);
	}
}
