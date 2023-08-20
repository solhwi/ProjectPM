using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UIManager : SingletonComponent<UIManager> 
{
	private EventSystem eventSystem;
	private Dictionary<Type, UIPopup> popupDictionary = new Dictionary<Type, UIPopup>();
	private Stack<UIPopup> popupStack = new Stack<UIPopup>();

	private UIMainWindow currentMainWindow = null;

	protected override void OnAwakeInstance()
	{
		FindMainWindow(SceneManager.Instance.CurrentSceneType);
		SceneManager.Instance.onSceneChanged += FindMainWindow;
		SceneManager.Instance.onSceneChanged += ReleasePopups;
	}

	protected override void OnReleaseInstance()
	{
		SceneManager.Instance.onSceneChanged -= FindMainWindow;
		SceneManager.Instance.onSceneChanged -= ReleasePopups;
	}

	private void FindMainWindow(SceneType type)
	{
		currentMainWindow = FindObjectOfType<UIMainWindow>();
		if(currentMainWindow != null)
		{
			currentMainWindow.SetOrder(0);
        }
		else
		{
			Debug.LogError($"{type} 씬에 Main Window가 존재하지 않습니다.");
		}

		eventSystem = FindObjectOfType<EventSystem>();
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

	public void OpenPopup<T>(UIParam param = null) where T : UIPopup
	{
		if (popupDictionary.TryGetValue(typeof(T), out var popup))
		{
			OpenPopup<T>(popup, param);
			return;
		}

		var popupPrefab = ResourceManager.Instance.Load<T>();
		if (popupPrefab == null)
		{
			StartCoroutine(LoadUIRoutine<T>(param));
		}
		else
		{
			var popupObj = Instantiate(popupPrefab);
			OpenPopup<T>(popupObj, param);
		}
	}

	private IEnumerator LoadUIRoutine<T>(UIParam param) where T : UIPopup
	{
		var handle = ResourceManager.Instance.LoadAsync<T>();
		while(!handle.IsDone || handle.Status != AsyncOperationStatus.Succeeded)
		{
			yield return null;
		}

		var popupPrefab = handle.Result as GameObject;
		if (popupPrefab == null)
			yield break;

		var popupObj = Instantiate(popupPrefab);
		if (popupObj == null)
			yield break;

		T popup  = popupObj.GetComponent<T>();
		if (popup == null)
			yield break;

		OpenPopup<T>(popup, param);
	}

	private void OpenPopup<T>(UIPopup popup, UIParam param)
	{
		if (popup.TryOpen(param))
		{
			popup.transform.SetParent(transform);
			popup.transform.SetPositionAndRotation(default, default);
			popup.transform.SetAsLastSibling();

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
}
