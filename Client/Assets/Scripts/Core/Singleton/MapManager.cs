using Cysharp.Threading.Tasks;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    private Dictionary<ENUM_MAP_TYPE, MapComponent> mapDictionary = new Dictionary<ENUM_MAP_TYPE, MapComponent>();
    private MapComponent currentMapComponent = null;

    private int lastMapOrder = 0;
        
    public async UniTask<MapComponent> CreateMap(ENUM_MAP_TYPE mapType)
    {
        MapComponent mapObject = null;

        switch (mapType)
        {
            case ENUM_MAP_TYPE.City:
                mapObject = await CreateMap<CityMapComponent>();
                break;
        }

        mapDictionary[mapType] = mapObject;
        return mapObject;
    }

    private async UniTask<T> CreateMap<T>() where T : MapComponent
    {
        var mapObject = await ResourceManager.Instance.InstantiateAsync<T>();

        mono.SetSingletonChild(this, mapObject);

        currentMapComponent = mapObject;
        lastMapOrder = mapObject.SetOrder(lastMapOrder);

        return mapObject;
    }

    public void MoveToSafeArea(MonoBehaviour obj)
    {
        if (currentMapComponent == null || obj == null)
        {
            Debug.LogError($"{(obj != null ? obj.name : "NULL")} 맵 배치 시도에 실패했습니다.");
            return;
        }

        currentMapComponent.MoveToSafeArea(obj);
    }

    public void MoveToMapArea(ENUM_TEAM_TYPE spawnType, MonoBehaviour obj)
    {
        if(currentMapComponent == null || obj == null)
        {
            Debug.LogError($"{spawnType} 타입의 {(obj != null ? obj.name : "NULL")} 맵 배치 시도에 실패했습니다.");
            return;
        }

        currentMapComponent.MoveToMapArea(spawnType, obj);
    }
}
