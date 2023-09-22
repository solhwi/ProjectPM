using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerHelper
{
    public static int GetSortingLayer(ENUM_LAYER_TYPE objectType, int orderIndex)
    {
        int layerNumber = (int)objectType;
        return layerNumber * 1000 + orderIndex;
    }
}
