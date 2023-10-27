using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleSessionManager : NetworkManager<BattleSessionManager>, ISessionComponent
{
	private MatchSceneModule.Param battleParam = null;
	private Coroutine sessionCoroutine = null;

	private int currentTickCount = 0;

	private WaitForSecondsRealtime serverTickFrequency = null;
	private float clientLatencyTime = 0.0f;

	public void Connect(SceneModuleParam param)
	{
		if (param is MatchSceneModule.Param battleParam)
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
