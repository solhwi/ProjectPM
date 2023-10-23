using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneModule : SceneModule
{
	public async override UniTask OnPrepareEnterRoutine(SceneModuleParam param)
	{
		await AddressableResourceSystem.Instance.LoadAsync<SettingPopup>();
    }
}
