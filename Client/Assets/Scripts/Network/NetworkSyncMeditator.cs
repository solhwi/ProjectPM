using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �� ������� ��ġ, ����, ��ų�� �����Ѵ�.
public struct NetworkFrameSyncData
{

}

// ���� ��ġ�� �̵��ϰ� �Ǹ�, �浹�� �߻��ϰ�, �浹�� �߻��ߴٸ� Hit State�� �ȴ�.
// �浹���� ���� ó������� �ұ�?

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
