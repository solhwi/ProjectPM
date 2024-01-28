using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PhysicsComponent : MonoBehaviour
{
	[SerializeField] private PhysicsSystem system;

    [Tooltip("The Layers which represent gameobjects that the Character Controller can be grounded on.")]
    [SerializeField] private LayerMask groundedLayerMask;

    [Tooltip("The distance down to check for ground.")]
    [SerializeField] private float groundedRaycastDistance = 0.1f;

    public bool UseGravity = true;

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

    public Vector2 Velocity => m_NextMovement;

    protected void Reset()
    {
		system = AssetLoadHelper.GetSystemAsset<PhysicsSystem>();
    }

    protected void OnEnable()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_BoxColider2D = GetComponent<BoxCollider2D>();

        m_CurrentPosition = m_Rigidbody2D.position;
        m_PreviousPosition = m_Rigidbody2D.position;

        m_ContactFilter.layerMask = groundedLayerMask;
        m_ContactFilter.useLayerMask = true;
        m_ContactFilter.useTriggers = false;

		Physics2D.queriesStartInColliders = false;

        system.Register(this);
    }

	protected void OnDisable()
	{
		if (system)
        {
            system.UnRegister(this);
        }
    }

	public void FlushMovement()
	{
		m_PreviousPosition = m_Rigidbody2D.position;
		m_CurrentPosition = m_PreviousPosition + m_NextMovement;

		m_NextMovement = Vector2.zero;
		m_Rigidbody2D.MovePosition(m_CurrentPosition);
	}

    public void AddMovement(Vector2 movement)
    {
		m_NextMovement += movement;
    }

	public void Teleport(Vector2 position)
    {
        Vector2 delta = position - m_CurrentPosition;
        m_PreviousPosition += delta;
        m_CurrentPosition = position;
        m_Rigidbody2D.MovePosition(position);
    }
     
    public void UpdateRaycastHit()
    {
		Vector2 raycastStart = m_Rigidbody2D.position + m_BoxColider2D.offset;
		float raycastDistance = m_BoxColider2D.size.x * 0.5f + groundedRaycastDistance * 2f;
		Vector2 raycastDirection = Vector2.down;
		Vector2 raycastStartBottomCentre = raycastStart + Vector2.down * (m_BoxColider2D.size.y * 0.5f - m_BoxColider2D.size.x * 0.5f);

		m_RaycastPositions[0] = raycastStartBottomCentre + Vector2.left * m_BoxColider2D.size.x * 0.5f;
		m_RaycastPositions[1] = raycastStartBottomCentre;
		m_RaycastPositions[2] = raycastStartBottomCentre + Vector2.right * m_BoxColider2D.size.x * 0.5f;

		for (int i = 0; i < m_RaycastPositions.Length; i++)
		{
			int count = Physics2D.Raycast(m_RaycastPositions[i], raycastDirection, m_ContactFilter, m_HitBuffer, raycastDistance);

			m_FoundHits[i] = count > 0 ? m_HitBuffer[0] : new RaycastHit2D();
			m_GroundColliders[i] = m_FoundHits[i].collider;
		}

		for (int i = 0; i < m_HitBuffer.Length; i++)
		{
			m_HitBuffer[i] = new RaycastHit2D();
		}
	}

	private Vector2 GetGroundNormal()
	{
		Vector2 groundNormal = Vector2.zero;
		int hitCount = 0;

		foreach(var foundHit in m_FoundHits)
		{
			if (foundHit.collider != null)
			{
				groundNormal += foundHit.normal;
				hitCount++;
			}
		}

		if (hitCount > 0)
		{
			groundNormal.Normalize();
		}
		
		return groundNormal;
	}

	public bool CheckGrounded()
    {
		Vector2 groundNormal = GetGroundNormal();
		bool isGrounded = false;
		
		if (Mathf.Approximately(groundNormal.x, 0f) && Mathf.Approximately(groundNormal.y, 0f))
		{
			isGrounded = false;
		}
		else
		{
			isGrounded = m_Rigidbody2D.velocity.y <= 0f;

			if (m_BoxColider2D != null)
			{
				if (m_GroundColliders[1] != null)
				{
					float capsuleBottomHeight = m_Rigidbody2D.position.y + m_BoxColider2D.offset.y - m_BoxColider2D.size.y * 0.5f;
					float middleHitHeight = m_FoundHits[1].point.y;
					isGrounded &= middleHitHeight < capsuleBottomHeight + groundedRaycastDistance;
				}
			}
		}

		return isGrounded;

	}
}