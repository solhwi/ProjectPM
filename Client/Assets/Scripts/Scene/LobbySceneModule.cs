using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

public class LobbySceneModule : SceneModule
{
	public async override UniTask OnPrepareEnterRoutine(SceneModuleParam param)
	{
		await ScriptParserManager.Instance.LoadAsyncScriptParsers();
        await AddressabeResourceSystem.Instance.LoadUnityAsset<RuntimeAnimatorController>("Assets/Bundle/Animation/RedMan/RedMan.overrideController");
		await AddressabeResourceSystem.Instance.LoadUnityAsset<BehaviourTree>("Assets/Bundle/AI/PencilMan.asset");
    }
}
