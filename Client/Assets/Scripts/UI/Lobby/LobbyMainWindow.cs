using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMainWindow : UIMainWindow
{
    public void OnClickBossMode()
    {
        BossSceneModuleParam param = new BossSceneModuleParam(ENUM_MAP_TYPE.City, ENUM_ENTITY_TYPE.RedMan);

        SceneModuleSystem.Instance.LoadScene(SceneType.Boss, param);
	}

    public void OnClickMatch()
    {
		SceneModuleSystem.Instance.LoadScene(SceneType.Match);
	}

    public void OnClickTrainingMode()
    {
		SceneModuleSystem.Instance.LoadScene(SceneType.Training);
	}
}
