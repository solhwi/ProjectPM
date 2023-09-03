using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IUpdater
{
	void OnUpdate(int deltaFrameCount, float deltaTime);
	void OnPostUpdate(int deltaFrameCount, float deltaTime);
	void OnLateUpdate(int deltaFrameCount, float deltaTime);
}

public abstract class SceneModule : MonoBehaviour, IUpdater
{
	[SerializeField] protected SceneType mySceneType = SceneType.Title;

#if UNITY_EDITOR
	protected virtual void Reset()
	{
        switch(this)
		{
			case TitleSceneModule:
				mySceneType = SceneType.Title;
                break;
            case LobbySceneModule:
                mySceneType = SceneType.Lobby;
                break;
            case BattleSceneModule:
                mySceneType = SceneType.Battle;
                break;
            case MatchSceneModule:
                mySceneType = SceneType.Match;
                break;
            case BossSceneModule:
                mySceneType = SceneType.Boss;
                break;
            case TrainingSceneModule:
                mySceneType = SceneType.Training;
                break;
        }
    }
#endif

	public virtual void OnEnter(SceneModuleParam param)
	{

	}

	public virtual IEnumerator OnPrepareEnterRoutine(SceneModuleParam param)
	{
		yield return null;
	}

	public virtual void OnExit()
	{

	}

	public virtual IEnumerator OnPrepareExitRoutine()
	{
		yield return null;
	}

	public virtual void OnUpdate(int deltaFrameCount, float deltaTime)
	{
		
	}

	public virtual void OnPostUpdate(int deltaFrameCount, float deltaTime)
	{
		
	}

	public virtual void OnLateUpdate(int deltaFrameCount, float deltaTime)
	{
		
	}
}
