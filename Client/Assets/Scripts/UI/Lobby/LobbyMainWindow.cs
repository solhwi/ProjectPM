using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMainWindow : UIMainWindow
{
    public void OnClickBossMode()
    {
        var param = new BossSceneModule.Param(ENUM_MAP_TYPE.City, ENUM_ENTITY_TYPE.RedMan);
        SceneModuleSystemManager.Instance.TryEnterSceneModule(SceneType.Boss, param);
	}

    public void OnClickMatch()
    {
		SceneModuleSystemManager.Instance.TryEnterSceneModule(SceneType.Match);
	}

    public void OnClickTrainingMode()
    {
		SceneModuleSystemManager.Instance.TryEnterSceneModule(SceneType.Training);
	}
}
