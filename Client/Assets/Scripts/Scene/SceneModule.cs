using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 씬에 하나씩 붙어있는 모듈이다.
/// 시스템에 가까운 기능을 하지만, 씬 모듈 시스템에 의해 관리되는 컴포넌트라는 개념
/// </summary>

public abstract class SceneModule : MonoComponent
{
	[SerializeField] protected SceneType mySceneType = SceneType.Title;
	protected float sceneOpenDeltaTime = 0.0f;

#if UNITY_EDITOR
	protected virtual void Reset()
	{
        switch(this)
		{
			case StartSceneModule:
				mySceneType = SceneType.Start;
				break;
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
		sceneOpenDeltaTime = 0.0f;
    }

    public async virtual UniTask OnPrepareEnterRoutine(SceneModuleParam param)
	{
		await UniTask.Yield();
    }

	public virtual void OnExit()
	{

	}

	public async virtual UniTask OnPrepareExitRoutine()
	{
        await UniTask.Yield();
    }

    public virtual void OnFixedUpdate(int deltaFrameCount, float deltaTime)
    {
        
    }

    public virtual void OnUpdate(int deltaFrameCount, float deltaTime)
	{
		sceneOpenDeltaTime += deltaTime;
    }

	public virtual void OnPostUpdate(int deltaFrameCount, float deltaTime)
	{
		
	}

	public virtual void OnLateUpdate(int deltaFrameCount, float deltaTime)
	{
		
	}

    protected virtual void OnDrawGizmos()
    {

    }

    public virtual void OnPrevUpdate(int deltaFrameCount, float deltaTime)
	{
		
	}
}
