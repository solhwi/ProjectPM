using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneModule : SceneModule
{
	public override IEnumerator OnPrepareEnterRoutine(SceneModuleParam param)
	{
		yield return ScriptParserManager.Instance.LoadAsyncScriptParsers();
		yield return ResourceManager.Instance.LoadAsync<SettingPopup>();
	}
}
