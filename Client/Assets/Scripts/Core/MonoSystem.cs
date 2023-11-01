using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AssetLoadHelper
{
    private const string SystemAssetPathFormat = "Assets/Bundle/System/{0}Asset.asset";
	private const string DataAssetPathFormat = "Assets/Bundle/Datas/Parser/{0}.asset";


	public static string GetSystemAssetPath<T>() where T : MonoSystem
    {
        return GetSystemAssetPath(typeof(T));
    }

    public static string GetSystemAssetPath(Type systemType)
    {
        return string.Format(SystemAssetPathFormat, systemType);
    }

	public static string GetDataAssetPath<T>() where T : ScriptParser
	{
		return GetDataAssetPath(typeof(T));
	}

	public static string GetDataAssetPath(Type dataType)
    {
        return string.Format(DataAssetPathFormat, dataType);
    }

    public static TSystem GetSystemAsset<TSystem>() where TSystem : MonoSystem
    {
        return AssetDatabase.LoadAssetAtPath<TSystem>(GetSystemAssetPath<TSystem>());
    }

	public static TParser GetDataAsset<TParser>() where TParser : ScriptParser
	{
		return AssetDatabase.LoadAssetAtPath<TParser>(GetDataAssetPath<TParser>());
	}
}

public static class SystemHelper
{
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
