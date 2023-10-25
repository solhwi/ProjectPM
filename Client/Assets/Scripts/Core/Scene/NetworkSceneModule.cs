using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;
using kcp2k;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;



public abstract class NetworkSceneModule : SceneModule
{
	protected ISessionComponent currentSession;
    protected Transport currentTransport;

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();

        ResetNetworkConfig();
    }

    private void ResetNetworkConfig()
    {
        var networkManager = CreateNetworkManager();
        var transport = CreateTransport();

        DefineConfig(networkManager, transport);

        EditorSceneManager.SaveScene(gameObject.scene);
    }

    protected virtual void DefineConfig(NetworkManager networkConfig, Transport transportConfig)
    {
        networkConfig.transport = transportConfig;
        networkConfig.dontDestroyOnLoad = false;
        networkConfig.runInBackground = true;
    }

    protected abstract NetworkManager CreateNetworkManager();
    protected abstract Transport CreateTransport();
#endif

    public override void OnEnter(SceneModuleParam param)
    {
        base.OnEnter(param);

        currentTransport = FindObjectOfType<Transport>();
        currentSession = FindObjectOfType<NetworkManager>() as ISessionComponent;

        try
        {
            currentTransport.Initialize();
            currentSession.Initialize();
        }
        catch (Exception e)
        {
            Debug.LogError($"{e} : {mySceneType}�� ��Ʈ��ũ �Ŵ��� Ȥ�� Ʈ������Ʈ�� �������� �ʽ��ϴ�.");
        }

        currentSession.Connect(param);
        
    }

	public override void OnExit()
    {
        base.OnExit();

        try
        {
            currentSession.Disconnect();
        }
        catch (Exception e)
        {
            Debug.LogError($"{e} : {mySceneType}�� ��Ʈ��ũ ������ �̹� ����Ǿ����ϴ�.");
        }

    }
}
