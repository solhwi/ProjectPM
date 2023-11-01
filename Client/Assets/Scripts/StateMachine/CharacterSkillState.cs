using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StateMachine
{
	public class CharacterSkillState : CharacterAnimatorState
    {
        [SerializeField] private SkillSystem skillSystem = null;
        private ENUM_SKILL_TYPE skillType = ENUM_SKILL_TYPE.None;

        protected override void Reset()
        {
            base.Reset();

            skillSystem = AssetLoadHelper.GetSystemAsset<SkillSystem>();
        }

        public override void OnSLStateEnter(CharacterBehaviour owner, FrameCommandMessage command)
		{
			skillType = characterSkillTable.GetSkillType(owner.EntityType, ENUM_ATTACK_KEY.SKILL);
			skillSystem.UseSkill(owner, skillType);
        }

		public override void OnSLStateNoTransitionUpdate(CharacterBehaviour owner, FrameCommandMessage command)
		{
			base.OnSLStateNoTransitionUpdate(owner, command);
		}

		public override void OnSLStateExit(CharacterBehaviour owner, FrameCommandMessage command)
		{
			base.OnSLStateExit(owner, command);
		}
	}

}
