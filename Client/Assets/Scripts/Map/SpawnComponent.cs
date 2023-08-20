using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnComponent : MonoBehaviour
{
    public ENUM_TEAM_TYPE TeamType => type;

    [SerializeField] private ENUM_TEAM_TYPE type = ENUM_TEAM_TYPE.Enemy;
}
