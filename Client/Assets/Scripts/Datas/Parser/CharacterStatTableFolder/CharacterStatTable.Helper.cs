using UnityEngine;

public partial class CharacterStatTable : ScriptParser
{
	public CharacterStat GetStat(ENUM_ENTITY_TYPE type)
	{
		if (characterStatDictionary.TryGetValue((int)type, out var stat))
		{
			return stat;
		}

		Debug.LogError($"{type}에 해당하는 정보가 없습니다.");
		return null;
	}
}
