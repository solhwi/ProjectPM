using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

public class LobbySceneModule : SceneModule
{
	public override IEnumerator OnPrepareEnterRoutine(SceneModuleParam param)
	{
		yield return ScriptParserManager.Instance.LoadAsyncScriptParsers();
        yield return ResourceManager.Instance.LoadUnityAsset<RuntimeAnimatorController>("Assets/Bundle/Animation/RedMan/RedMan.overrideController");
		yield return ResourceManager.Instance.LoadUnityAsset<BehaviourTree>("Assets/Bundle/AI/PencilMan.asset");
    }
}
