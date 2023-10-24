using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 사실상 어트리뷰트 경로를 위해 분리되는 클래스이다.
/// 맵은 코드보다 프리팹 자체가 중요함
/// </summary>

[Map("CityMap.prefab")]
public class CityMapComponent : MapComponent
{
    public override ENUM_MAP_TYPE MapType => ENUM_MAP_TYPE.City;
}
