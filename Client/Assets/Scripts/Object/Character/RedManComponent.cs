using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Character("NormalCharacter.prefab")]
public class RedManComponent : EntityMeditatorComponent
{
	public override ENUM_ENTITY_TYPE EntityType => ENUM_ENTITY_TYPE.RedMan;
}
