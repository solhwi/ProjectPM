using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using System.Linq;

public class FindEnemy : ActionNode
{
    protected override void OnStart() 
    {

    }

    protected override void OnStop() 
    {

    }

    protected override State OnUpdate() 
    {
        blackboard.searchedEnemieDictionary.Clear();

        var searchedEnemiesDictionary = EntityManager.Instance.GetSearchedEntities(context.entityComponent);
        if (searchedEnemiesDictionary == null || searchedEnemiesDictionary.Any() == false)
            return State.Failure;

        foreach(var enemyPair in searchedEnemiesDictionary)
        {
            blackboard.searchedEnemieDictionary.Add(enemyPair.Key, enemyPair.Value);
        }

        return State.Success;
    }
}
