using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    public IEnumerable<EnemySpawnData> GetCurrentStageEnemies()
    {
        // OrderBy 세팅 한 번
        var spawnData = new EnemySpawnData();
        spawnData.spawnTime = 5.0f;
        spawnData.enemyGuid = 123;
        spawnData.entityType = ENUM_ENTITY_TYPE.PencilMan;
        yield return spawnData;
    }

    public EnemySpawnData GetCurrentStageBoss()
    {
        var spawnData = new EnemySpawnData();
        spawnData.spawnTime = 10.0f;
        spawnData.enemyGuid = 124;
        spawnData.entityType = ENUM_ENTITY_TYPE.PencilMan;
        return spawnData;
    }
}
