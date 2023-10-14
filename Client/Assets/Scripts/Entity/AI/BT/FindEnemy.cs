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
        var searchedEnemiesDictionary = EntityManager.Instance.GetSearchedEntities(context.entityComponent);
        if (searchedEnemiesDictionary == null)
            return State.Failure;

        blackboard.searchedEnemieDictionary = (SearchEnemyDictionary)searchedEnemiesDictionary;
        return State.Success;
    }
}
