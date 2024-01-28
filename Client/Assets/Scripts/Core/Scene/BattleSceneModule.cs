using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneModule : NetworkSceneModule
{
	public class Param : SceneModuleParam
	{
		public readonly bool isOwner = false;

		public Param(bool isOwner)
		{
			this.isOwner = isOwner;
		}
	}
}
