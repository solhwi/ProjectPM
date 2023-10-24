using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawnSubSystem
{
	[System.Serializable]
	private class TeamPositionDictionary : SerializableDictionary<ENUM_TEAM_TYPE, List<TeamPositionComponent>> { }
	[SerializeField] private TeamPositionDictionary teamPositionDictionary = new();

	public void Initialize(MapComponent mapComponent)
    {
		foreach (var component in mapComponent.GetComponentsInChildren<TeamPositionComponent>())
		{
			if (teamPositionDictionary.TryGetValue(component.TeamType, out var positionList))
			{
				if (positionList == null)
					positionList = new List<TeamPositionComponent>();

				positionList.Add(component);
			}
		}
	}

	public void Spawn(IEntity entity)
	{
		if (teamPositionDictionary.TryGetValue(entity.TeamType, out var positionList))
		{
			positionList[0].SetPosition(entity);
		}
	}
}
