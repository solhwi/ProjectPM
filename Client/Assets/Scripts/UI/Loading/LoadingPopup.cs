using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[PopupAttribute("LoadingPopup.prefab")]
public class LoadingPopup : UIPopup
{
    [SerializeField] private Image backgroundImage = null;
    [SerializeField] private Image progressBar = null;

    protected override void OnOpen(UIParam param = null)
    {
        base.OnOpen(param);
        backgroundImage.raycastTarget = true;
    }

    protected override void OnClose()
    {
        base.OnClose();
        progressBar.fillAmount = 0.0f;
    }

    public void SetProgress(float progress)
    {
        progressBar.fillAmount = progress;
    }
}
