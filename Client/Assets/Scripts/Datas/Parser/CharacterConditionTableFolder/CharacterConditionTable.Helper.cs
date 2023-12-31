﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class CharacterConditionTable : ScriptParser
{
    private Dictionary<string, IStateCondition> conditionDictionary = new Dictionary<string, IStateCondition>();
    private const char ParameterSeparator = ':';
    private const char ConditionAndSeparator = '*';
    private const char ConditionOrSeparator = '+';


    public IStateCondition GetCondition(string rawConditionType)
    {
        if (conditionDictionary.TryGetValue(rawConditionType, out var condition))
        {
            return condition;
        }

        if (rawConditionType.Contains(ParameterSeparator) == false)
        {
            Debug.LogError($"정의되지 않은 컨디션 타입 : {rawConditionType}");
        }

        return null;
    }


    public override void RuntimeParser()
    {
        MakeConditions();
    }

    public void MakeConditions()
    {
        foreach (var rawCondition in rawConditionList)
        {
            var conditionType = rawCondition.key.Trim();
            var condition = MakeStateCondition(conditionType, out string[] parameters);
            if (condition != null)
            {
                condition.Parse(rawCondition.condition);
                conditionDictionary.Add(conditionType, condition);
            }
        }
    }

    public IEnumerable<KeyValuePair<IStateCondition, bool>> ParseStateConditions(string compositeRawCondition)
    {
        foreach (var orSeparatedCondition in compositeRawCondition.Split(ConditionOrSeparator))
        {
            string[] andSeparatedConditions = orSeparatedCondition.Split(ConditionAndSeparator);

            for (int i = 0; i < andSeparatedConditions.Length; i++)
            {
                var rawCondition = andSeparatedConditions[i];

                var stateCondition = GetCondition(rawCondition);
                if (stateCondition == null)
                {
                    stateCondition = MakeStateCondition(rawCondition, out string[] parameters);
                    stateCondition.Parse(parameters);
                }

                yield return new KeyValuePair<IStateCondition, bool>(stateCondition, i > 0);
            }
        }
    }

    private IStateCondition MakeStateCondition(string rawCondition, out string[] parameters)
    {
        string[] rawConditions = rawCondition.Split(ParameterSeparator);

        string inputRawConditionType = rawConditions[0];
        parameters = rawConditions.Where(raw => raw.Equals(inputRawConditionType) == false).ToArray();

        switch (inputRawConditionType)
        {
            case "[AnimationWaitTime]":
                return new AnimationWaitCondition();

            case "[PressAttack]":
                return new AttackCondition();

            case "[GoUp]":
                return new GoUpCondition();

            case "[FallDown]":
                return new FallDownCondition();

            case "[PressGuard]":
                return new PressGuardCondition();

            case "[PressMove]":
                return new MoveCondition();

            case "[PressDash]":
                return new DashCondition();

            case "[Grounded]":
                return new GroundedCondition();

            case "[Damage]":
                return new DamageCondition();

            case "[PressJump]":
                return new PressJumpCondition();

            case "[Combo]":
            case "[JumpUp]":
            case "[JumpDown]":
            case "[Dash]":
            case "[PressSkill]":
            case "[PressUltimate]":
                return new CompositeStateCondition(this);

            default:
                return null;
        }
    }
}
