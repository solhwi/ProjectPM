using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

public class LobbySceneModule : SceneModule
{
	public async override UniTask OnPrepareEnterRoutine(SceneModuleParam param)
	{
		await ScriptParsingSystem.Instance.LoadAsyncScriptParsers();
        await AddressableResourceSystem.Instance.LoadUnityAsset<RuntimeAnimatorController>("Assets/Bundle/Animation/RedMan/RedMan.overrideController");
		await AddressableResourceSystem.Instance.LoadUnityAsset<BehaviourTree>("Assets/Bundle/AI/PencilMan.asset");
    }
}
