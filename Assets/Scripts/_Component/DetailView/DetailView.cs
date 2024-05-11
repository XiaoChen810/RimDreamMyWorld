using ChenChen_BuildingSystem;
using ChenChen_UISystem;
using UnityEngine;


/// <summary>
/// 细节视图，展示细节
/// </summary>
public abstract class DetailView : MonoBehaviour
{
    public bool OnShow = false;
    //public DetailViewPanel Panel { get; protected set; }

    public virtual void Selected()
    {
        Debug.Log($"{gameObject.name} is selected");
        AddPanel();
    }

    protected abstract void AddPanel();

    protected abstract void UpdateShow(DetailViewPanel panel);

    protected virtual void Update()
    {
        if(OnShow)
        {
            DetailViewPanel detail = PanelManager.Instance.GetTopPanel() as DetailViewPanel;
            if(detail != null)
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
