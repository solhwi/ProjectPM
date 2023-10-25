using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMainWindow : UIMainWindow
{
    public void OnClickBossMode()
    {
        BossSceneModuleParam param = new BossSceneModuleParam(ENUM_MAP_TYPE.City, ENUM_ENTITY_TYPE.RedMan);

        SceneModuleSystemManager.Instance.LoadScene(SceneType.Boss, param);
	}

    public void OnClickMatch()
    {
		SceneModuleSystemManager.Instance.LoadScene(SceneType.Match);
	}

    public void OnClickTrainingMode()
    {
		SceneModuleSystemManager.Instance.LoadScene(SceneType.Training);
	}
}
