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
            Debug.LogError($"{e} : {mySceneType}�� ��Ʈ��ũ �Ŵ��� Ȥ�� Ʈ������Ʈ�� �������� �ʽ��ϴ�.");
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
            Debug.LogError($"{e} : {mySceneType}�� ��Ʈ��ũ ������ �̹� ����Ǿ����ϴ�.");
        }
    }
}
