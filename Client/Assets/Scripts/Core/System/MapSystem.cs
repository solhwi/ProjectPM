using Cysharp.Threading.Tasks;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSystem : MonoSystem<MapSystem>
{
    private MapSpawnSubSystem spawn = new MapSpawnSubSystem();

	public async UniTask CreateMap(ENUM_MAP_TYPE mapType)
	{
		var mapComponent = await InternalCreateMap(mapType);
        if (mapComponent == null)
            return;

        spawn.Initialize(mapComponent);
	}

    public void Spawn(IEntity entity)
    {
        spawn?.Spawn(entity);
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
        var mapObject = await AddressableResourceSystem.Instance.InstantiateAsync<T>();

		mapObject.SetOrderLayer();
		behaviour.SetSystemChild(this, mapObject);

        return mapObject;
    }
}
