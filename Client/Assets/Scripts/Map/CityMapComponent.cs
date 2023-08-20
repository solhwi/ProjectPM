using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Map("CityMap.prefab")]
public class CityMapComponent : MapComponent
{
    protected override void Reset()
    {
        base.Reset();
        mapType = ENUM_MAP_TYPE.City;
    }

    protected override IEnumerator Start()
    {
        yield return null;
    }
}
