using ChenChen_UISystem;
using System;

/// <summary>
/// 单独提供一个UI面板保证只会有一个视图
/// </summary>
public class DetailViewManager : Singleton<DetailViewManager>
{
    private PanelManager _panelManager;
    public PanelManager PanelManager
    {
        get
        {
            if (_panelManager == null)
            {
                _panelManager = new PanelManager();
            }
            return _panelManager;
        }
    }
}

