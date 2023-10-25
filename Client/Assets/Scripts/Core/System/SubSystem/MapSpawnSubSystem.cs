using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawnSubSystem : MonoSystem
{
	[System.Serializable]
	private class TeamPositionDictionary : SerializableDictionary<ENUM_TEAM_TYPE, List<TeamPositionComponent>> { }
	[SerializeField] private TeamPositionDictionary teamPositionDictionary = new TeamPositionDictionary();

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
		if(entity == null)
		{
			Debug.LogError($"��ȯ�� ��ƼƼ�� NULL�Դϴ�.");
			return;
		}

		if (teamPositionDictionary.TryGetValue(entity.TeamType, out var positionList))
		{
			positionList[0].SetPosition(entity);
		}
	}
}
