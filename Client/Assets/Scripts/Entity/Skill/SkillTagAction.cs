using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ISkillTagAction
{
    public bool Parse(params string[] rawParameters);
    public void Trigger(AddressableResourceSystem resourceSystem);
}

public class CompositeSkillTagAction : ISkillTagAction
{
    private CharacterSkillTable skillTable = null;
    private List<ISkillTagAction> skillTagActions = new List<ISkillTagAction>();

    public CompositeSkillTagAction(CharacterSkillTable table)
    {
        skillTable = table;
    }

    public bool Parse(params string[] compositeRawCondition)
    {
        if (compositeRawCondition.Any() == false)
            return false;

        skillTagActions = skillTable.ParseSkillTagActions(compositeRawCondition[0]).ToList();
        return true;
    }

    public void Trigger(AddressableResourceSystem resourceSystem)
    {
        foreach (var skillTagAction in skillTagActions)
        {
            skillTagAction.Trigger(resourceSystem);
        }
    }
}


public abstract class StringTagAction : ISkillTagAction
{
    protected string value = string.Empty;

    public bool Parse(params string[] rawParameters)
    {
        if (rawParameters.Any() == false)
            return false;

        value = rawParameters[0];
        return true;
    }

    public abstract void Trigger(AddressableResourceSystem resourceSystem);
}

public class InstantiateTagAction : StringTagAction
{
    public override void Trigger(AddressableResourceSystem resourceSystem)
    {
        resourceSystem.InstantiateAsync(value).Forget();
    }
}