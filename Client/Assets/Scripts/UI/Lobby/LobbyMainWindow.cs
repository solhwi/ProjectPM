using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMainWindow : UIMainWindow
{
    public void OnClickBossMode()
    {
        BossSceneModuleParam param = new BossSceneModuleParam(ENUM_MAP_TYPE.City, ENUM_ENTITY_TYPE.RedMan);

        SceneManager.Instance.LoadScene(SceneType.Boss, param);
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
