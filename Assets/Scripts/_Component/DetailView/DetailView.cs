using ChenChen_BuildingSystem;
using ChenChen_UISystem;
using UnityEngine;


/// <summary>
/// ϸ����ͼ��չʾϸ��
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
    }

    public void EndShow()
    {
        onShow = false;
    }
}
