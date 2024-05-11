using ChenChen_UISystem;
using System;

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

