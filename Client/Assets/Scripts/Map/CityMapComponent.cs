using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ǻ� ��Ʈ����Ʈ ��θ� ���� �и��Ǵ� Ŭ�����̴�.
/// ���� �ڵ庸�� ������ ��ü�� �߿���
/// </summary>

[Map("CityMap.prefab")]
public class CityMapComponent : MapComponent
{
    public override ENUM_MAP_TYPE MapType => ENUM_MAP_TYPE.City;
}
