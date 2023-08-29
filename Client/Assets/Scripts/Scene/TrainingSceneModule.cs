using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingSceneModule : SceneModule
{
	public override void OnUpdate()
	{
		InputManager.Instance.OnUpdate();
		PhysicsManager.Instance.OnUpdate();
	}
}
