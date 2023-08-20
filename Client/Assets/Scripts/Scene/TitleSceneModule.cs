using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneModule : SceneModule
{
	public override IEnumerator OnPrepareEnterRoutine(SceneModuleParam param)
	{
		return ResourceManager.Instance.LoadAsync<SettingPopup>();
	}
}
