using Cysharp.Threading.Tasks;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public class AddressableResourceSystem : MonoSystem<AddressableResourceSystem>
{
	private class ObjectPathData
	{
		public readonly string Path;
		public readonly Object Obj;

		public ObjectPathData(string path, Object obj)
		{
			Path = path;
			Obj = obj;
		}
	}

	private Dictionary<Type, List<ObjectPathData>> resourceDictionary = new Dictionary<Type, List<ObjectPathData>>();

    public T LoadCached<T>(string path = default) where T : Object
	{
		if (resourceDictionary.TryGetValue(typeof(T), out var list))
		{
            ObjectPathData data = path == default ? list.FirstOrDefault() : list.Find(res => res.Path == path);
            if (data == null)
				return null;

			return data.Obj as T;
        }

		return null;
	}

	public async UniTask<TParentClass> LoadAsync<TParentClass>(Type subclassType) where TParentClass : Object
	{
		string path = FMUtil.GetResourcePath(subclassType);
		if (string.IsNullOrEmpty(path))
			return default;

		var resourceType = FMUtil.GetResourceType(subclassType);
        return await LoadAsync<TParentClass>(resourceType, path);
    }

	private async UniTask<T> LoadAsync<T>(ResourceType resourceType, string path) where T : Object
    {
        switch (resourceType)
        {
            case ResourceType.UnityAsset:
                return await LoadUnityAsset<T>(path);

            case ResourceType.Prefab:
                var prefab = await LoadPrefab<T>(path);

				if (typeof(T).IsSubclassOf(typeof(Component)) == false)
				{
					Debug.LogError($"{typeof(T)} is not Unity Component");
					return null;
                }

                return prefab.GetComponent<T>();
        }

		return default;
    }

	public async UniTask<T> InstantiateAsync<T>(Transform parent = null) where T : MonoBehaviour
	{
        string path = FMUtil.GetResourcePath<T>();
        if (string.IsNullOrEmpty(path))
            return default;

		if (resourceDictionary.TryGetValue(typeof(T), out var objectPathData))
			return objectPathData.FirstOrDefault().Obj as T;

        var go = await InstantiateAsync<T>(path, parent);
		return go.GetComponent<T>();
    }

	public async UniTask<T> LoadAsync<T>() where T : Object
	{
		string path = FMUtil.GetResourcePath<T>();
		if (string.IsNullOrEmpty(path))
			return default;

		var resourceType = FMUtil.GetResourceType<T>();
		return await LoadAsync<T>(resourceType, path);
    }
	
	private async UniTask<GameObject> InstantiateAsync<T>(string path, Transform parent = null) where T : Object
	{
        var handle = Addressables.InstantiateAsync(path, parent);

        handle.Completed += (op) =>
        {
            AddResource(op, path);
        };

        await handle.Task;
        return handle.Result;
    }


	public async UniTask<T> LoadUnityAsset<T>(string path) where T : Object
	{
		var handle = Addressables.LoadAssetAsync<T>(path);

		handle.Completed += (op) =>
		{
            AddResource(op, path);
        };

		return await handle;
	}

	private void AddResource<T>(AsyncOperationHandle<T> op, string path) where T : Object
	{
        if (op.IsDone && op.Status == AsyncOperationStatus.Succeeded)
        {
            if (resourceDictionary.ContainsKey(typeof(T)) == false)
                resourceDictionary.Add(typeof(T), new List<ObjectPathData>());

            var data = new ObjectPathData(path, op.Result);
            resourceDictionary[typeof(T)].Add(data);
        }
    }

	private async UniTask<GameObject> LoadPrefab<T>(string path) where T : Object
	{
		var handle = Addressables.LoadAssetAsync<GameObject>(path);

		handle.Completed += (op) =>
		{
			AddResource(op, path);
        };

		return await handle;
	}
}
