using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PhysicsComponent : MonoBehaviour
{
    [Tooltip("The Layers which represent gameobjects that the Character Controller can be grounded on.")]
    [SerializeField] private LayerMask groundedLayerMask;

    [Tooltip("The distance down to check for ground.")]
    [SerializeField] private float groundedRaycastDistance = 0.1f;

    [Tooltip("Use Gravity Toggle")]
    [SerializeField] private bool useGravity;

	Rigidbody2D m_Rigidbody2D;
    BoxCollider2D m_BoxColider2D;
    Vector2 m_PreviousPosition;
    Vector2 m_CurrentPosition;
    Vector2 m_NextMovement;
    ContactFilter2D m_ContactFilter;
    RaycastHit2D[] m_HitBuffer = new RaycastHit2D[5];
    RaycastHit2D[] m_FoundHits = new RaycastHit2D[3];
    Collider2D[] m_GroundColliders = new Collider2D[3];
    Vector2[] m_RaycastPositions = new Vector2[3];

    public Vector2 HitBox => m_BoxColider2D.size;
    public Vector2 HitOffset => m_BoxColider2D.offset;

    public bool IsGrounded { get; private set; }
	private float airborneDeltaTime = 0.0f;

    public Vector2 Velocity => m_NextMovement;

	private void OnEnable()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_BoxColider2D = GetComponent<BoxCollider2D>();

        m_CurrentPosition = m_Rigidbody2D.position;
        m_PreviousPosition = m_Rigidbody2D.position;

        m_ContactFilter.layerMask = groundedLayerMask;
        m_ContactFilter.useLayerMask = true;
        m_ContactFilter.useTriggers = false;

		Physics2D.queriesStartInColliders = false;
        PhysicsManager.Instance.Register(this);
    }

	private void OnDisable()
	{
		PhysicsManager.Instance.UnRegister(this);
	}

	public void OnFixedUpdate(int deltaFrameCount, float deltaTime)
    {
        CheckCapsuleEndCollisions(true); // 충돌을 체크한다.

        AddGravity(deltaTime); // 중력을 넣는다.
        FlushMovement(); // 모두 수행
	}

    public void OnUpdate(int deltaFrameCount, float deltaTime)
    {
        
    }

    private void AddGravity(float deltaTime)
    {
		if (useGravity == false)
			return;

		if (IsGrounded == false)
		{
			AddMovement(PhysicsManager.Instance.GetMovementByGravity(airborneDeltaTime));
			airborneDeltaTime += deltaTime;
		}
		else
		{
            TeleportToGround(m_FoundHits[1].point);
            airborneDeltaTime = 0.0f;
		}
	}

	private void FlushMovement()
    {
		m_PreviousPosition = m_Rigidbody2D.position;
		m_CurrentPosition = m_PreviousPosition + m_NextMovement;

		m_NextMovement = Vector2.zero;
		m_Rigidbody2D.MovePosition(m_CurrentPosition);
	}

    /// <summary>
    /// This moves a rigidbody and so should only be called from FixedUpdate or other Physics messages.
    /// </summary>
    /// <param name="movement">The amount moved in global coordinates relative to the rigidbody2D's position.</param>
    public void AddMovement(Vector2 movement)
    {
		m_NextMovement += movement;
    }

	/// <summary>
	/// This moves the character without any implied velocity.
	/// </summary>
	/// <param name="position">The new position of the character in global space.</param>
	public void Teleport(Vector2 position)
    {
        Vector2 delta = position - m_CurrentPosition;
        m_PreviousPosition += delta;
        m_CurrentPosition = position;
        m_Rigidbody2D.MovePosition(position);
    }
     
    private void TeleportToGround(Vector2 groundPos)
    {
        Teleport(new Vector2(groundPos.x, groundPos.y + m_BoxColider2D.size.y * 0.5f + groundedRaycastDistance));
    }

    /// <summary>
    /// This updates the state of IsGrounded.  It is called automatically in FixedUpdate but can be called more frequently if higher accurracy is required.
    /// </summary>
    private void CheckCapsuleEndCollisions(bool bottom)
    {
        Vector2 raycastDirection;
        Vector2 raycastStart;
        float raycastDistance;

        if (m_BoxColider2D == null)
        {
            raycastStart = m_Rigidbody2D.position + Vector2.up;
            raycastDistance = 1f + groundedRaycastDistance;

            if (bottom)
            {
                raycastDirection = Vector2.down;

                m_RaycastPositions[0] = raycastStart + Vector2.left * 0.4f;
                m_RaycastPositions[1] = raycastStart;
                m_RaycastPositions[2] = raycastStart + Vector2.right * 0.4f;
            }
            else
            {
                raycastDirection = Vector2.up;

                m_RaycastPositions[0] = raycastStart + Vector2.left * 0.4f;
                m_RaycastPositions[1] = raycastStart;
                m_RaycastPositions[2] = raycastStart + Vector2.right * 0.4f;
            }
        }
        else
        {
            raycastStart = m_Rigidbody2D.position + m_BoxColider2D.offset;
            raycastDistance = m_BoxColider2D.size.x * 0.5f + groundedRaycastDistance * 2f;

            if (bottom)
            {
                raycastDirection = Vector2.down;
                Vector2 raycastStartBottomCentre = raycastStart + Vector2.down * (m_BoxColider2D.size.y * 0.5f - m_BoxColider2D.size.x * 0.5f);

                m_RaycastPositions[0] = raycastStartBottomCentre + Vector2.left * m_BoxColider2D.size.x * 0.5f;
                m_RaycastPositions[1] = raycastStartBottomCentre;
                m_RaycastPositions[2] = raycastStartBottomCentre + Vector2.right * m_BoxColider2D.size.x * 0.5f;
            }
            else
            {
                raycastDirection = Vector2.up;
                Vector2 raycastStartTopCentre = raycastStart + Vector2.up * (m_BoxColider2D.size.y * 0.5f - m_BoxColider2D.size.x * 0.5f);

                m_RaycastPositions[0] = raycastStartTopCentre + Vector2.left * m_BoxColider2D.size.x * 0.5f;
                m_RaycastPositions[1] = raycastStartTopCentre;
                m_RaycastPositions[2] = raycastStartTopCentre + Vector2.right * m_BoxColider2D.size.x * 0.5f;
            }
        }

        for (int i = 0; i < m_RaycastPositions.Length; i++)
        {
            // Debug.DrawRay(m_RaycastPositions[i], raycastDirection, Color.red);
            int count = Physics2D.Raycast(m_RaycastPositions[i], raycastDirection, m_ContactFilter, m_HitBuffer, raycastDistance);

            if (bottom)
            {
                m_FoundHits[i] = count > 0 ? m_HitBuffer[0] : new RaycastHit2D();
                m_GroundColliders[i] = m_FoundHits[i].collider;
            }
        }

        if (bottom)
        {
            Vector2 groundNormal = Vector2.zero;
            int hitCount = 0;

            for (int i = 0; i < m_FoundHits.Length; i++)
            {
                if (m_FoundHits[i].collider != null)
                {
                    groundNormal += m_FoundHits[i].normal;
                    hitCount++;
                }
            }

            if (hitCount > 0)
            {
                groundNormal.Normalize();
            }

            if (Mathf.Approximately(groundNormal.x, 0f) && Mathf.Approximately(groundNormal.y, 0f))
            {
                IsGrounded = false;
            }
            else
            {
                bool isPrevGrounded = IsGrounded;
                IsGrounded = m_Rigidbody2D.velocity.y <= 0f;

                if (m_BoxColider2D != null)
                {
                    if (m_GroundColliders[1] != null)
                    {
                        float capsuleBottomHeight = m_Rigidbody2D.position.y + m_BoxColider2D.offset.y - m_BoxColider2D.size.y * 0.5f;
                        float middleHitHeight = m_FoundHits[1].point.y;
                        IsGrounded &= middleHitHeight < capsuleBottomHeight + groundedRaycastDistance;
                    }
                }

              
            }
        }

        for (int i = 0; i < m_HitBuffer.Length; i++)
        {
            m_HitBuffer[i] = new RaycastHit2D();
        }
    }
}