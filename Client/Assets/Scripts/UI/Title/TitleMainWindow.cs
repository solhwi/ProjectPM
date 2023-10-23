using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleMainWindow : UIMainWindow
{
	public void OnClickStart()
	{
		SceneModuleSystem.Instance.LoadScene(SceneType.Lobby);
	}

	public void OnClickSetting()
	{
		PrefabLinkedUISystem.Instance.OpenPopupAsync<SettingPopup>().Forget();
	}
}
