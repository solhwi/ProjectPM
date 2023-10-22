using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UIManager : Singleton<UIManager> 
{
	private EventSystem eventSystem;
	private Dictionary<Type, UIPopup> popupDictionary = new Dictionary<Type, UIPopup>();
	private Stack<UIPopup> popupStack = new Stack<UIPopup>();

	private UIMainWindow currentMainWindow = null;

	protected override void OnAwakeInstance()
	{
		FindMainWindow(SceneManager.Instance.CurrentSceneType);
		SceneManager.Instance.onSceneChanged += FindMainWindow;
		// SceneManager.Instance.onSceneChanged += ReleasePopups;
	}

	protected override void OnReleaseInstance()
	{
		SceneManager.Instance.onSceneChanged -= FindMainWindow;
		// SceneManager.Instance.onSceneChanged -= ReleasePopups;
	}

	public void BlockAllUI()
	{
		if(eventSystem)
            eventSystem.gameObject.SetActive(false);
    }

	public void UnBlockAllUI()
	{
		//if(eventSystem)
		//	eventSystem.gameObject.SetActive(true);
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

	public async void OpenPopupAsync<T>(UIParam param = null) where T : UIPopup
	{
		if (popupDictionary.TryGetValue(typeof(T), out var popup))
		{
			OpenPopup<T>(popup, param);
			return;
		}

		var popupPrefab = ResourceManager.Instance.Load<T>();
		if (popupPrefab == null)
		{
			await CreatePopup<T>(param);
        }
		else
		{
			var popupObj = UnityEngine.Object.Instantiate(popupPrefab);
			OpenPopup<T>(popupObj, param);
		}
	}

	private async UniTask<T> CreatePopup<T>(UIParam param) where T : UIPopup
	{
		var popup = await ResourceManager.Instance.InstantiateAsync<T>();
		OpenPopup<T>(popup, param);

		return popup;
	}

	private void OpenPopup<T>(UIPopup popup, UIParam param)
	{
		if (popup.TryOpen(param))
		{
            mono.SetSingletonChild(this, popup);

            popup.SetOrder(popupStack.Count + 1);
			popupStack.Push(popup);
		}

		popupDictionary[typeof(T)] = popup;
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
