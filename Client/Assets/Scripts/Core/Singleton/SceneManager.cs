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

public class SceneManager : Singleton<SceneManager>
{
    public event Action<SceneType> onSceneChanged = null;

	public SceneType CurrentSceneType => currentSceneType;
	private SceneType currentSceneType;

    private SceneModule currentSceneModule = null;
	private SceneModuleParam currentParam = null;

    private Coroutine sceneLoadCoroutine = null;
	private bool isLoadComplete = false;

	protected override void OnAwakeInstance()
	{
		currentSceneModule = UnityEngine.Object.FindObjectOfType<SceneModule>();
		if(currentSceneModule != null)
			currentSceneModule.OnEnter(null);

		mono.onFixedUpdate += FixedUpdate;
		mono.onUpdate += Update;
		mono.onLateUpdate += LateUpdate;

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

	protected override void OnReleaseInstance()
	{
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;

		mono.onFixedUpdate -= FixedUpdate;
        mono.onUpdate -= Update;
        mono.onLateUpdate -= LateUpdate;

        if (sceneLoadCoroutine != null)
            mono.StopCoroutine(sceneLoadCoroutine);
    }

	private void FixedUpdate()
	{
		if (currentSceneModule != null)
			currentSceneModule.OnFixedUpdate(Time.frameCount, Time.deltaTime);
    }

	private void Update()
	{
		if (isLoadComplete == false)
			return;

		if (currentSceneModule != null)
			currentSceneModule.OnPrevUpdate(Time.frameCount, Time.deltaTime);

		if (currentSceneModule != null)
			currentSceneModule.OnUpdate(Time.frameCount, Time.deltaTime);

		if (currentSceneModule != null)
			currentSceneModule.OnPostUpdate(Time.frameCount, Time.deltaTime);
	}

	private void LateUpdate()
    {
        if (isLoadComplete == false)
            return;

        if (currentSceneModule != null)
			currentSceneModule.OnLateUpdate(Time.frameCount, Time.deltaTime);
	}

	public void LoadScene(SceneType sceneType, SceneModuleParam param = null)
	{
		currentParam = param;
        sceneLoadCoroutine = mono.StartCoroutine(OnLoadSceneCoroutine(sceneType, null));
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMoad)
	{
		if(Enum.TryParse(scene.name, out currentSceneType))
		{
            onSceneChanged?.Invoke(currentSceneType);
            sceneLoadCoroutine = mono.StartCoroutine(OnCompletedSceneCoroutine());
		}
    }

    private IEnumerator OnLoadSceneCoroutine(SceneType sceneType, Action<float> OnSceneLoading)
	{
		yield return currentSceneModule?.OnPrepareExitRoutine();

		currentSceneModule?.OnExit();

		isLoadComplete = false;

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
        currentSceneModule = UnityEngine.Object.FindObjectOfType<SceneModule>();
		if (currentSceneModule == null)
			yield break;

        yield return currentSceneModule.OnPrepareEnterRoutine(currentParam);
        currentSceneModule.OnEnter(currentParam);

        currentParam = null;

        yield return null;
		isLoadComplete = true;
    }
}
