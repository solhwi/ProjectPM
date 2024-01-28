using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleMainWindow : UIMainWindow
{
	public void OnClickStart()
	{
		SceneModuleSystemManager.Instance.TryEnterSceneModule(SceneType.Match);
	}

	public void OnClickSetting()
	{
		uiSystem.OpenPopupAsync<SettingPopup>().Forget();
	}
}
