using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PrefabLinkedUISystem : MonoSystem
{
	[SerializeField] private AddressableResourceSystem resourceSystem = null;

    private EventSystem eventSystem;
	private Dictionary<Type, UIPopup> popupDictionary = new Dictionary<Type, UIPopup>();
	private Stack<UIPopup> popupStack = new Stack<UIPopup>();

	private UIMainWindow currentMainWindow = null;

    protected override void OnReset()
    {
        base.OnReset();

        resourceSystem = AssetLoadHelper.GetSystemAsset<AddressableResourceSystem>();
    }

    public override void OnEnter()
	{
		FindMainWindow(SceneModuleSystemManager.Instance.CurrentSceneType);
		SceneModuleSystemManager.Instance.onSceneChanged += FindMainWindow;
	}

    public override void OnExit()
	{
		SceneModuleSystemManager.Instance.onSceneChanged -= FindMainWindow;
	}

	private void FindMainWindow(SceneType type)
	{
		currentMainWindow = UnityEngine.Object.FindObjectOfType<UIMainWindow>();
		if(currentMainWindow != null)
		{
			currentMainWindow.SetOrder(0);
        }
		else
		{
			Debug.LogError($"{type} 씬에 Main Window가 존재하지 않습니다.");
		}

		eventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();
		if (eventSystem == null)
		{
			Debug.LogError($"{type} 씬에 Event System이 존재하지 않습니다.");
		}
	}

	private void ReleasePopups(SceneType type)
	{
		popupStack.Clear();
		popupDictionary.Clear();
	}

	public async UniTask<T> OpenPopupAsync<T>(UIParam param = null) where T : UIPopup
	{
		if (popupDictionary.TryGetValue(typeof(T), out var popup))
		{
			return OpenPopup<T>(popup, param);
		}

        popup = await CreatePopup<T>();
		return OpenPopup<T>(popup, param);
    }

	private async UniTask<T> CreatePopup<T>() where T : UIPopup
	{
		return await resourceSystem.InstantiateAsync<T>();
	}

	private T OpenPopup<T>(UIPopup popup, UIParam param) where T : UIPopup
	{
		if (popup.TryOpen(param))
		{
			this.SetChildObject(popup);

			popup.SetOrder(popupStack.Count + 1);
			popupStack.Push(popup);
		}

		popupDictionary[typeof(T)] = popup;
		return popup as T;
	}

	public void ClosePopup<T>() where T : UIPopup
	{
		if (popupDictionary.TryGetValue(typeof(T), out var popup) == false)
			return;

		if (popup.TryClose() == false)
			return;

		Stack<UIPopup> tempStack = new Stack<UIPopup>();

		while(popupStack.Count > 0)
		{
			var peekPopup = popupStack.Pop();
			if (peekPopup == null)
				continue;

			if (peekPopup.GetType() == typeof(T))
				break;

			tempStack.Push(peekPopup);
		}

		while(tempStack.Count > 0)
		{
			popupStack.Push(tempStack.Pop());
		}
	}

	public void ClosePopup(Type type)
	{
		if (popupDictionary.TryGetValue(type, out var popup) == false)
			return;

		if (popup.TryClose() == false)
			return;

		Stack<UIPopup> tempStack = new Stack<UIPopup>();

		while (popupStack.Count > 0)
		{
			var peekPopup = popupStack.Pop();
			if (peekPopup == null)
				continue;

			if (peekPopup.GetType() == type)
				break;

			tempStack.Push(peekPopup);
		}

		while (tempStack.Count > 0)
		{
			popupStack.Push(tempStack.Pop());
		}
	}

	public T GetPopup<T>(bool includeDeactive = false) where T : UIPopup
	{
        if (popupDictionary.TryGetValue(typeof(T), out var popup))
        {
            var _popup = popup as T;
            if (_popup != null)
			{
				if (_popup.IsActive || includeDeactive)
				{
					return _popup;
                }
			}
        }

		return null;
    }
}
