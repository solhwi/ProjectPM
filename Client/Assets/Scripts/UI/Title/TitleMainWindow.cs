using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleMainWindow : UIMainWindow
{
	public void OnClickStart()
	{
		SceneManager.Instance.LoadScene(SceneType.Select);
	}

	public void OnClickSetting()
	{
		UIManager.Instance.OpenPopup<SettingPopup>();
	}
}
