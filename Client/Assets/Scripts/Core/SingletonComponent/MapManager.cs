using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MapManager : SingletonComponent<MapManager>
{
    private Dictionary<Type, MapComponent> mapDictionary = new Dictionary<Type, MapComponent>();
    private MapComponent currentMapComponent = null;

    private int lastMapOrder = 0;
        
    public IEnumerator LoadAsyncMap(ENUM_MAP_TYPE mapType)
    {
        switch(mapType)
        {
            case ENUM_MAP_TYPE.City:
                yield return LoadMapRoutine<CityMapComponent>();
                break;
        }
    }

    private IEnumerator LoadMapRoutine<T>() where T : MapComponent
    {
        var handle = ResourceManager.Instance.LoadAsync<T>();
        while (!handle.IsDone || handle.Status != AsyncOperationStatus.Succeeded)
        {
            yield return null;
        }

        var prefab = handle.Result as GameObject;
        if (prefab == null)
            yield break;

        var obj = Instantiate(prefab);
        if (obj == null)
            yield break;

        T map = obj.GetComponent<T>();
        if (map == null)
            yield break;

        currentMapComponent = map;
        mapDictionary[typeof(T)] = map;

        map.transform.SetParent(transform);
        map.transform.SetPositionAndRotation(default, default);
        map.transform.SetAsLastSibling();

        lastMapOrder = map.SetOrder(lastMapOrder);
    }

    public IEnumerator UnloadAsyncMap()
    {
        yield return null;
    }

    public void MoveToMapArea<T>(ENUM_TEAM_TYPE spawnType, T obj) where T : MonoBehaviour
    {
        if(currentMapComponent == null)
        {
            Debug.LogError($"{spawnType} 타입의 {obj.name} 맵 배치 시도에 실패했습니다.");
            return;
        }

        currentMapComponent.MoveToMapArea<T>(spawnType, obj);
    }
}
