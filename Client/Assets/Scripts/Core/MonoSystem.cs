using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class SystemHelper
{
    private const string SystemAssetPathFormat = "Assets/Bundle/System/{0}Asset.asset";

    public static string GetSystemAssetPath<T>() where T : MonoSystem
    {
        return GetSystemAssetPath(typeof(T));
    }

    public static string GetSystemAssetPath(Type systemType)
    {
        return string.Format(SystemAssetPathFormat, systemType);
    }

    public static TSystem GetSystemAsset<TSystem>() where TSystem : MonoSystem
    {
        return AssetDatabase.LoadAssetAtPath<TSystem>(GetSystemAssetPath<TSystem>());
    }

    public static void SetChildObject(this MonoSystem system, MonoBehaviour childObj)
    {
		SceneModuleSystemManager.Instance.SetSystemChild(system, childObj);
	}
}

public abstract class MonoSystem : ScriptableObject
{
    private void Reset()
    {
        OnReset();
    }

    protected virtual void OnReset()
    {

    }

    public virtual void OnEnter()
    {

    }

    public virtual void OnExit()
    {

    }

    public virtual void OnUpdate(int deltaFrameCount, float deltaTime)
    {
        
    }
}
