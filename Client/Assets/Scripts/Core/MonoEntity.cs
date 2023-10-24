using UnityEngine;

public enum ENUM_ENTITY_TYPE
{
	None = -1,
	RedMan = 0,
	BlueMan = 1,
	GreenMan = 2,
	PencilMan = 3,
}

public interface IEntity
{
	int CurrentState
	{
		get;
	}

	Vector2 Velocity
	{
		get;
	}

	Vector2 HitBox
	{
		get;
	}

	Vector2 HitOffset
	{
		get;
	}

	bool IsLeftDirection
	{
		get;
	}

	Vector2 Position
	{
		get;
	}

	bool IsGrounded
	{
		get;
	}

	float CurrentNormalizedTime
	{
		get;
	}

	ENUM_ENTITY_TYPE EntityType
	{
		get;
	}

	ENUM_TEAM_TYPE TeamType
	{
		get;
	}

	bool IsPlayer
	{
		get;
	}

	bool IsBoss
	{
		get;
	}

	int OwnerGuid
	{
		get;
	}

	int EntityGuid
	{
		get;
	}

	//bool IsAlive
	//{
	//	get;
	//}

	void SetPosition(Vector2 position);
	void PushCommand(ICommand command);
}