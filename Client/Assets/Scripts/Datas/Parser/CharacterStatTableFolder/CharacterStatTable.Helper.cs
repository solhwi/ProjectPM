using UnityEngine;

public partial class CharacterStatTable : ScriptParser
{
	public CharacterStat GetStat(ENUM_CHARACTER_TYPE type)
	{
		if (CharacterStatDictionary.TryGetValue((int)type, out var stat))
		{
			return stat;
		}

		Debug.LogError($"{type}�� �ش��ϴ� ������ �����ϴ�.");
		return null;
	}
}
