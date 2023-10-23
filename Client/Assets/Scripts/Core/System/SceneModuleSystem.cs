using Cysharp.Threading.Tasks;
using Mono.CecilX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    Start = -1,
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

public class SceneModuleSystem : Singleton<SceneModuleSystem>
{
    public event Action<SceneType> onSceneChanged = null;

    public SceneType CurrentSceneType => currentSceneType;
    private SceneType currentSceneType;

    private SceneModule currentSceneModule = null;
    private SceneModuleParam currentParam = null;

    private CancellationTokenSource cancellationSource = new CancellationTokenSource();
    private bool isLoadComplete = false;

    protected override void OnAwakeInstance()
    {
        currentSceneModule = UnityEngine.Object.FindObjectOfType<SceneModule>();
        if (currentSceneModule != null)
            currentSceneModule.OnEnter(currentParam);

        mono.onFixedUpdate += FixedUpdate;
        mono.onUpdate += Update;
        mono.onLateUpdate += LateUpdate;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void OnReleaseInstance()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;

        mono.onFixedUpdate -= FixedUpdate;
        mono.onUpdate -= Update;
        mono.onLateUpdate -= LateUpdate;

        if (cancellationSource != null)
			cancellationSource.Cancel();
	}

    private void FixedUpdate()
    {
        if (currentSceneModule != null)
            currentSceneModule.OnFixedUpdate(Time.frameCount, Time.fixedDeltaTime);
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
        StartLoadSceneAsync(sceneType).ToCancellationToken(cancellationSource.Token);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMoad)
    {
        if (Enum.TryParse(scene.name, out currentSceneType))
        {
            onSceneChanged?.Invoke(currentSceneType);
            FinishLoadSceneAsync().ToCancellationToken(cancellationSource.Token);
        }
    }

    private async UniTask StartLoadSceneAsync(SceneType sceneType)
    {
        if(currentSceneModule)
        {
            await currentSceneModule.OnPrepareExitRoutine();
            currentSceneModule?.OnExit();
        }

        isLoadComplete = false;
        var popup = await PrefabLinkedUISystem.Instance.OpenPopupAsync<LoadingPopup>();
        if (popup == null)
            return;

        await LoadSceneAsync(sceneType, popup.SetProgress);
    }

    private async UniTask LoadSceneAsync(SceneType sceneType, Action<float> OnSceneLoading)
    {
        var operation = SceneManager.LoadSceneAsync(sceneType.ToString(), LoadSceneMode.Single);
        operation.allowSceneActivation = false;
        await operation.ToUniTask(Progress.Create((float progress) =>
        {
            if (progress >= 0.9f && operation.allowSceneActivation == false)
            {
				operation.allowSceneActivation = true;
				OnSceneLoading?.Invoke(1.0f);
			}
			else
            {
				OnSceneLoading?.Invoke(progress);
			}
		}));
    }

    private async UniTask FinishLoadSceneAsync()
    {
        currentSceneModule = UnityEngine.Object.FindObjectOfType<SceneModule>();
        if (currentSceneModule == null)
            return;

        await currentSceneModule.OnPrepareEnterRoutine(currentParam);
        currentSceneModule.OnEnter(currentParam);

        currentParam = null;
        isLoadComplete = true;

        PrefabLinkedUISystem.Instance.ClosePopup<LoadingPopup>();
    }
}
