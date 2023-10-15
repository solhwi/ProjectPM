using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneModule : SceneModule
{
    private void Start()
    {
        SceneManager.Instance.LoadScene(SceneType.Title);
    }
}
