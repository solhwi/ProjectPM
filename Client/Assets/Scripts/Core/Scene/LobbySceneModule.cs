using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

public class LobbySceneModule : SceneModule
{
	public async override UniTask OnPrepareEnterRoutine(SceneModuleParam param)
	{
		await scriptParsingSystem.LoadAsyncScriptParsers();
        await resourceSystem.LoadUnityAsset<RuntimeAnimatorController>("Assets/Bundle/Animation/RedMan/RedMan.overrideController");
		await resourceSystem.LoadUnityAsset<BehaviourTree>("Assets/Bundle/AI/PencilMan.asset");
    }
}
