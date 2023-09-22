using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.Examples.TMP_ExampleScript_01;

public enum ENUM_MAP_TYPE
{
    City = 0,
}

public abstract class MapComponent : MonoBehaviour
{
    [SerializeField] protected ENUM_MAP_TYPE mapType;
    [SerializeField] protected SpriteRenderer[] spriteRenderers;

    [SerializeField] private SpawnComponent friendlySpawnArea = null;
    [SerializeField] private List<SpawnComponent> enemySpawnAreas = new List<SpawnComponent>();

    protected virtual void Reset()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        foreach(var component in GetComponentsInChildren<SpawnComponent>())
        {
            if(component.TeamType == ENUM_TEAM_TYPE.Friendly)
            {
                friendlySpawnArea = component;
            }
            else
            {
                enemySpawnAreas.Add(component);
            }
        }

        gameObject.layer = (int)ENUM_LAYER_TYPE.Map;
    }

    protected abstract IEnumerator Start();

    public int SetOrder(int order)
    {
        foreach(var renderer in spriteRenderers)
        {
            renderer.sortingOrder = LayerHelper.GetSortingLayer(ENUM_LAYER_TYPE.Map, order++);
        }

        return order;
    }

    public void MoveToMapArea<T>(ENUM_TEAM_TYPE spawnType, T obj) where T : MonoBehaviour
    {
        if(spawnType == ENUM_TEAM_TYPE.Friendly)
        {
            obj.transform.position = friendlySpawnArea.transform.position;
        }
        else
        {
            int index = Random.Range(0, enemySpawnAreas.Count);
            obj.transform.position = enemySpawnAreas[index].transform.position;
        }
    }
}
