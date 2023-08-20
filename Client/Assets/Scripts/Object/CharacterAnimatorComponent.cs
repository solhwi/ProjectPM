using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimatorComponent : AnimatorComponent<CharacterState>
{
	protected readonly int m_HashHorizontalSpeedPara = Animator.StringToHash("HorizontalSpeed");
	protected readonly int m_HashVerticalSpeedPara = Animator.StringToHash("VerticalSpeed");
	protected readonly int m_HashGroundedPara = Animator.StringToHash("Grounded");
	protected readonly int m_HashCrouchingPara = Animator.StringToHash("Crouching");
	protected readonly int m_HashPushingPara = Animator.StringToHash("Pushing");
	protected readonly int m_HashTimeoutPara = Animator.StringToHash("Timeout");
	protected readonly int m_HashRespawnPara = Animator.StringToHash("Respawn");
	protected readonly int m_HashDeadPara = Animator.StringToHash("Dead");
	protected readonly int m_HashHurtPara = Animator.StringToHash("Hurt");
	protected readonly int m_HashForcedRespawnPara = Animator.StringToHash("ForcedRespawn");
	protected readonly int m_HashMeleeAttackPara = Animator.StringToHash("MeleeAttack");
	protected readonly int m_HashHoldingGunPara = Animator.StringToHash("HoldingGun");
	protected readonly int m_HashAirborneMeleeAttackState = Animator.StringToHash("AirborneMeleeAttack");
}
