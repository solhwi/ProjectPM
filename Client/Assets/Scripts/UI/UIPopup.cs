using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIParam
{

}


/// <summary>
/// �ڵ带 ���� ���������� ���� ���� �� �ִ� UI�� Popup�� ��ӹ޴´�.
/// </summary>

public class UIPopup : UIWindow
{
    public bool IsActive
    {
        get;
        private set;
    }

    public bool TryOpen(UIParam param = null)
    {
        if (IsActive)
            return false;

        gameObject.SetActive(true);
        IsActive = true;

        OnOpen(param);
        return true;
    }

    public bool TryClose()
    {
        if (!IsActive)
            return false;

        gameObject.SetActive(false);
        IsActive = false;

        OnClose();
        return true;
    }

    protected virtual void OnOpen(UIParam param = null)
    {

    }

    protected virtual void OnClose()
    {

    }

    public void OnClickClose()
    {
        PrefabLinkedUISystem.Instance.ClosePopup(GetType());
    }
}

