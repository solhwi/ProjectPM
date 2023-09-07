using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Character("NormalCharacter.prefab")]
public class NormalCharacterComponent : CharacterComponent
{
	public override ENUM_CHARACTER_TYPE CharacterType => ENUM_CHARACTER_TYPE.Normal;
}
