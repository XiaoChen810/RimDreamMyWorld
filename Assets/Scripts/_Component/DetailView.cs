using ChenChen_BuildingSystem;
using ChenChen_UISystem;
using UnityEngine;


/// <summary>
/// 细节视图，展示细节
/// </summary>
public abstract class DetailView : MonoBehaviour
{
    public bool onShow = false;

    protected virtual void OnMouseDown()
    {
        Debug.Log($"{gameObject.name} is clicked");
        AddPanel();
    }

    protected abstract void AddPanel();

    protected abstract void UpdateShow(DetailViewPanel panel);

    protected virtual void Update()
    {
        if(onShow)
        {
            DetailViewPanel detail = PanelManager.Instance.GetTopPanel() as DetailViewPanel;
            if(detail != null)
            {
                UpdateShow(detail);
            }
        }
    }

    public void StartShow()
    {
        onShow = true;
        Debug.Log($"{(gameObject != null ? gameObject.name : null)} is on show");
    }

    public void EndShow()
    {
        onShow = false;
        Debug.Log($"{(gameObject != null ? gameObject.name : null)} stop show");
    }
}
