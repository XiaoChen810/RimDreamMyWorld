using ChenChen_BuildingSystem;
using ChenChen_UISystem;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 细节视图，展示细节
/// </summary>
public abstract class DetailView : MonoBehaviour
{
    public bool OnShow = false;

    /// <summary>
    /// 内容
    /// </summary>
    public List<string> Content = new List<string>();

    public virtual void Selected()
    {
        AddPanel();
    }

    protected abstract void AddPanel();

    protected abstract void UpdateShow(DetailViewPanel panel);

    protected virtual void Update()
    {
        if(OnShow)
        {
            if (DetailViewManager.Instance.PanelManager.GetTopPanel() is DetailViewPanel detail)
            {
                UpdateShow(detail);
            }
        }
    }

    public virtual void StartShow()
    {
        OnShow = true;
    }

    public virtual void EndShow()
    {
        OnShow = false;
    }
}
