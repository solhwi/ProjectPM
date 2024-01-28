using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;



public abstract class NetworkSceneModule : SceneModule
{
    public override void OnEnter(SceneModuleParam param)
    {
        base.OnEnter(param);

        try
        {
            
        }
        catch (Exception e)
        {
            Debug.LogError($"{e} : {mySceneType}에 네트워크 매니저 혹은 트랜스포트가 존재하지 않습니다.");
        }
        
    }

	public override void OnExit()
    {
        base.OnExit();

        try
        {
            
        }
        catch (Exception e)
        {
            Debug.LogError($"{e} : {mySceneType}의 네트워크 세션이 이미 종료되었습니다.");
        }
    }
}
