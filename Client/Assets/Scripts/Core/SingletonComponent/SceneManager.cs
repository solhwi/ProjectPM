using Mono.CecilX;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
	Title = 0,
	Lobby = 1,
	Boss = 2,
	Match = 3,
	Battle = 4,
	Training = 5,
}

public class SceneModuleParam
{

}

public class SceneManager : SingletonComponent<SceneManager>
{
    public event Action<SceneType> onSceneChanged = null;

	public SceneType CurrentSceneType => currentSceneType;
	private SceneType currentSceneType;

    private SceneModule currentSceneModule = null;
	private SceneModuleParam currentParam = null;

    private Coroutine sceneLoadCoroutine = null;

	protected override void OnAwakeInstance()
	{
		currentSceneModule = FindObjectOfType<SceneModule>();
		if(currentSceneModule != null)
			currentSceneModule.OnEnter(null);

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

	protected override void OnReleaseInstance()
	{
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;

		if(sceneLoadCoroutine != null)
			StopCoroutine(sceneLoadCoroutine);
    }

	private void Update()
	{
		if (currentSceneModule != null)
			currentSceneModule.OnUpdate();
	}

	public void LoadScene(SceneType sceneType, SceneModuleParam param = null)
	{
		currentParam = param;
        sceneLoadCoroutine = StartCoroutine(OnLoadSceneCoroutine(sceneType, null));
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMoad)
	{
		if(Enum.TryParse(scene.name, out currentSceneType))
		{
            onSceneChanged?.Invoke(currentSceneType);
            sceneLoadCoroutine = StartCoroutine(OnCompletedSceneCoroutine());
		}
    }

    private IEnumerator OnLoadSceneCoroutine(SceneType sceneType, Action<float> OnSceneLoading)
	{
		yield return currentSceneModule?.OnPrepareExitRoutine();

		currentSceneModule?.OnExit();

		var asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneType.ToString(), LoadSceneMode.Single);

		asyncOperation.allowSceneActivation = false;

		while (asyncOperation.progress < 0.9f)
		{
			yield return null;

			OnSceneLoading?.Invoke(asyncOperation.progress);
		}

        asyncOperation.allowSceneActivation = true;
	}

	private IEnumerator OnCompletedSceneCoroutine()
    {
        currentSceneModule = FindObjectOfType<SceneModule>();
		if (currentSceneModule == null)
			yield break;

        yield return currentSceneModule.OnPrepareEnterRoutine(currentParam);
        currentSceneModule.OnEnter(currentParam);

        currentParam = null;
    }
}
