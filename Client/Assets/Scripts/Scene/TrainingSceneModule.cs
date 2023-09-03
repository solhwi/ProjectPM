using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingSceneModule : SceneModule
{
	public override void OnUpdate(int deltaFrameCount, float deltaTime)
	{
		InputManager.Instance.OnUpdate(deltaFrameCount, deltaTime);
		PhysicsManager.Instance.OnUpdate(deltaFrameCount, deltaTime);
	}

	public override void OnPostUpdate(int deltaFrameCount, float deltaTime)
	{
		ObjectManager.Instance.OnPostUpdate(deltaFrameCount, deltaTime);
	}

	public override void OnLateUpdate(int deltaFrameCount, float deltaTime)
	{
        ObjectManager.Instance.OnLateUpdate(deltaFrameCount, deltaTime);
	}
}
