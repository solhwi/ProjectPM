using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneModule : MonoBehaviour
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
            case SelectSceneModule:
                mySceneType = SceneType.Select;
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

	public virtual void OnUpdate()
	{

	}
}
