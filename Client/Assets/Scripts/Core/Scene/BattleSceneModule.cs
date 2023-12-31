using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneModule : SceneModule
{
    [SerializeField] protected MapSystem mapSystem = null;
    [SerializeField] protected PhysicsSystem physicsSystem = null;
    [SerializeField] protected EntitySystem entitySystem = null;
    [SerializeField] protected SkillSystem skillSystem = null;

    protected override void Reset()
    {
        base.Reset();

        mapSystem = AssetLoadHelper.GetSystemAsset<MapSystem>();
        physicsSystem = AssetLoadHelper.GetSystemAsset<PhysicsSystem>();  
        entitySystem = AssetLoadHelper.GetSystemAsset<EntitySystem>();
        skillSystem = AssetLoadHelper.GetSystemAsset<SkillSystem>();
    }
}
