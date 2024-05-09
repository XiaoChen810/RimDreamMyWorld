using ChenChen_UISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorTool : MonoBehaviour
{
    public void Animation_LoadingScene()
    {
        PanelManager.Instance.AddPanel(new LoadingPanel(true), false);
    }

    public void Animation_EndLoadingScene()
    {
        PanelManager.Instance.AddPanel(new LoadingPanel(false));
    }
}
