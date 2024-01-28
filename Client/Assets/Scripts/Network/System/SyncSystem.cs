using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncSystem : MonoSystem, IPacketReceiver
{
	[SerializeField] private SessionSystem sessionSystem = null;

	public override void OnEnter()
	{
		base.OnEnter();

		sessionSystem.RegisterPacketReceiver(this);
	}

	public override void OnExit()
	{
		base.OnExit();

		sessionSystem.UnRegisterPacketReceiver(this);
	}

	public void Send(IPacket packet)
	{
		sessionSystem.Send(packet);
	}

	public void OnReceive(IPacket packet)
	{
		
	}
}
