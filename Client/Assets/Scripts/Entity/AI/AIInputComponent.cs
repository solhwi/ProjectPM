using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENUM_AI_TYPE
{
	NormalMonster = 0,
}

public class AIInputComponent : InputComponent
{
	public ENUM_AI_TYPE AIType
	{
		get
		{
			return aiType;
		}
	}
	private ENUM_AI_TYPE aiType;

    public override void OnUpdate(int deltaFrameCount, float deltaTime)
    {
		// AI ·ÎÁ÷
    }
}
