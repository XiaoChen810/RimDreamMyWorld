using ChenChen_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatorTool : MonoBehaviour
{
    public void Animation_LoadingScene()
    {
        PanelManager.Instance.AddPanel(new Panel_Loading(true, this), false);
    }

    public void Animation_EndLoadingScene()
    {
        PanelManager.Instance.AddPanel(new Panel_Loading(false, this));
    }
}
