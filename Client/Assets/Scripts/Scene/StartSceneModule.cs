using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneModule : SceneModule
{
    private void Start()
    {
        SceneModuleSystem.Instance.LoadScene(SceneType.Title);
    }
}
