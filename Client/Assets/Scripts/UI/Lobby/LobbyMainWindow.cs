using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMainWindow : UIMainWindow
{
    public void OnClickBossMode()
    {
		SceneManager.Instance.LoadScene(SceneType.Boss);
	}

    public void OnClickMatch()
    {
		SceneManager.Instance.LoadScene(SceneType.Match);
	}

    public void OnClickTrainingMode()
    {
		SceneManager.Instance.LoadScene(SceneType.Training);
	}
}
