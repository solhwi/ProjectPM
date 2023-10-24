using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PrefabLinkedUISystem : MonoSystem<PrefabLinkedUISystem> 
{
	private EventSystem eventSystem;
	private Dictionary<Type, UIPopup> popupDictionary = new Dictionary<Type, UIPopup>();
	private Stack<UIPopup> popupStack = new Stack<UIPopup>();

	private UIMainWindow currentMainWindow = null;

	protected override void OnInitializeSystem()
	{
		FindMainWindow(SceneModuleSystem.Instance.CurrentSceneType);
		SceneModuleSystem.Instance.onSceneChanged += FindMainWindow;
	}

	protected override void OnReleaseSystem()
	{
		SceneModuleSystem.Instance.onSceneChanged -= FindMainWindow;
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
			Debug.LogError($"{type} ���� Main Window�� �������� �ʽ��ϴ�.");
		}

		eventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();
		if (eventSystem == null)
		{
			Debug.LogError($"{type} ���� Event System�� �������� �ʽ��ϴ�.");
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
		return await AddressableResourceSystem.Instance.InstantiateAsync<T>();
	}

	private T OpenPopup<T>(UIPopup popup, UIParam param) where T : UIPopup
	{
		if (popup.TryOpen(param))
		{
            behaviour.SetSystemChild(this, popup);

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
