using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSystem : MonoSystem
{
    [System.Serializable]
    private class SkillDictionary : SerializableDictionary<ENUM_SKILL_TYPE, Skill> {}

    [SerializeField] private SkillDictionary skillDictionary = new SkillDictionary();

    // 스킬 인포 테이블을 통하여 실제 스킬 딕셔너리를 제작한다.

    // 스킬 인포는 스킬의 타입, 쿨타임, 적 서치 박스, 스킬 액션 태그 등으로 이루어져 있다.

    // 스킬은 태그를 통해 제작된 스킬 액션 컴포넌트의 집합체이다.

    // 스킬 스테이트 머신에선 자신의 ATTACK_1와 ENTITY TYPE을 통해 실제 스킬을 가져올 수 있다.

    // 특정 앤티티는 자신을 시스템에 등록하여, 시스템이 자신의 스킬의 쿨타임 등 체크를 할 수 있도록 한다.
}
