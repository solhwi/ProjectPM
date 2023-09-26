using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerHelper
{
    public static int GetSortingLayer(ENUM_LAYER_TYPE layerType, int orderIndex)
    {
        int layerNumber = (int)layerType;
        return layerNumber * 1000 + Mathf.Abs(orderIndex % 1000);
    }

    public static LayerMask GetLayerMask(ENUM_LAYER_TYPE layerType)
    {
        return LayerMask.NameToLayer(layerType.ToString());
    }
}
