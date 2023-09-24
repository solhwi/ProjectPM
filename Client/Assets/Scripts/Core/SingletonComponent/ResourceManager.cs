using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public class ResourceManager : Singleton<ResourceManager>
{
    Dictionary<Type, List<Object>> resourceDictionary = new Dictionary<Type, List<Object>>();

	public T Load<T>() where T : Object
	{
		if(resourceDictionary.TryGetValue(typeof(T), out var list))
		{
			var obj = list.First();
			if (obj != null)
			{
				return obj as T;
			}
		}

		return null;
	}

	public T LoadForce<T>(string path) where T : Object
	{
		return Resources.Load<T>(path);
	}

	public T InstantiateForce<T>(T prefab) where T : Object
	{
		return UnityEngine.Object.Instantiate<T>(prefab);
	}

	public AsyncOperationHandle LoadAsync<TParentClass>(Type subclassType) where TParentClass : Object
	{
		string path = FMUtil.GetResourcePath(subclassType);
		if (string.IsNullOrEmpty(path))
			return default;

		var resourceType = FMUtil.GetResourceType(subclassType);

		switch (resourceType)
		{
			case ResourceType.UnityAsset:
				return LoadUnityAsset<TParentClass>(path);

			case ResourceType.Prefab:
				return LoadPrefab<TParentClass>(path);
		}

		return default;
	}

	public AsyncOperationHandle LoadAsync<T>() where T : Object
	{
		string path = FMUtil.GetResourcePath<T>();
		if (string.IsNullOrEmpty(path))
			return default;

		var resourceType = FMUtil.GetResourceType<T>();

		switch(resourceType)
		{
			case ResourceType.UnityAsset:
				return LoadUnityAsset<T>(path);

			case ResourceType.Prefab:
				return LoadPrefab<T>(path);
		}

		return default;
    }

	private AsyncOperationHandle<T> LoadUnityAsset<T>(string path) where T : Object
	{
		var handle = Addressables.LoadAssetAsync<T>(path);

		handle.Completed += (op) =>
		{
			if (op.IsDone && op.Status == AsyncOperationStatus.Succeeded)
			{
                if (resourceDictionary.ContainsKey(typeof(T)) == false)
					resourceDictionary.Add(typeof(T), new List<Object>());

				resourceDictionary[typeof(T)].Add(op.Result);
			}
		};

		return handle;
	}

	private AsyncOperationHandle LoadPrefab<T>(string path) where T : Object
	{
		var handle = Addressables.LoadAssetAsync<GameObject>(path);

		handle.Completed += (op) =>
		{
			if (op.IsDone && op.Status == AsyncOperationStatus.Succeeded)
			{
				if (resourceDictionary.ContainsKey(typeof(T)) == false)
					resourceDictionary.Add(typeof(T), new List<Object>());

				resourceDictionary[typeof(T)].Add(op.Result);
			}
		};

		return handle;
	}
}
