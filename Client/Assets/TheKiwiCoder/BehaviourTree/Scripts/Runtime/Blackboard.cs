using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using static CharacterSkillTable;

namespace TheKiwiCoder {

    [System.Serializable]
    public class SearchEnemyDictionary : SerializableDictionary<ENUM_SKILL_TYPE, IEnumerable<IEntity>> { }

    // This is the blackboard container shared between all nodes.
    // Use this to store temporary data that multiple nodes need read and write access to.
    // Add other properties here that make sense for your specific use case.
    [System.Serializable]
    public class Blackboard {
        public SearchEnemyDictionary searchedEnemieDictionary = new SearchEnemyDictionary();
    }
}