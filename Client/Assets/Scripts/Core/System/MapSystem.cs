using Cysharp.Threading.Tasks;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSystem : MonoSystem
{
    [SerializeField] private AddressableResourceSystem resourceSystem = null;
    [SerializeField] private MapSpawnSubSystem mapSpawnSubSystem;

    protected override void OnReset()
    {
        base.OnReset();

        resourceSystem = SystemHelper.GetSystemAsset<AddressableResourceSystem>();
        mapSpawnSubSystem = SystemHelper.GetSystemAsset<MapSpawnSubSystem>();
    }

    public async UniTask CreateMap(ENUM_MAP_TYPE mapType)
	{
		var mapComponent = await InternalCreateMap(mapType);
        if (mapComponent == null)
            return;

        mapSpawnSubSystem.Initialize(mapComponent);
	}

    public void Spawn(IEntity entity)
    {
        mapSpawnSubSystem?.Spawn(entity);
    }

	private async UniTask<MapComponent> InternalCreateMap(ENUM_MAP_TYPE mapType)
    {
        switch (mapType)
        {
            case ENUM_MAP_TYPE.City:
				return await CreateMap<CityMapComponent>();
        }

        return null;
    }

    private async UniTask<T> CreateMap<T>() where T : MapComponent
    {
        var mapObject = await resourceSystem.InstantiateAsync<T>();

		mapObject.SetOrderLayer();
		this.SetChildObject(mapObject);

		return mapObject;
    }
}
