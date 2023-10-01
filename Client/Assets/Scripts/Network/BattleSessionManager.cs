using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleSessionManager : NetworkManager<BattleSessionManager>, ISessionComponent
{
	private BattleSceneModuleParam battleParam = null;

	private Coroutine sessionCoroutine = null;
	private bool IsServerSession = false;

	public void Connect(SceneModuleParam param)
	{
		if (param is BattleSceneModuleParam battleParam)
		{
			this.battleParam = battleParam;

			if (battleParam.isOwner)
			{
				StartHost();
			}
			else
			{
				StartClient();
			}
		}
	}

	public void Disconnect()
	{
		if (battleParam.isOwner)
		{
			StopHost();
		}
		else
		{
			StopClient();
		}
	}

}
