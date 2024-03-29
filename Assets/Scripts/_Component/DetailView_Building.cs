using ChenChen_BuildingSystem;
using ChenChen_UISystem;
using UnityEngine;
using UnityEngine.UI;

public class DetailView_Building : DetailView
{
    protected Building building;

    private void OnEnable()
    {
        building = GetComponent<Building>();
    }

    protected override void OnMouseDown()
    {
        if (BuildingSystemManager.Instance.OnBuildingMode) return;
        base.OnMouseDown();
    }

    protected override void UpdateShow(DetailViewPanel panel)
    {
        if (panel == null) return;
        if (building == null) return;

        panel.SetView(
            building.Data.Name,
            building.MaxDurability,
            building.CurDurability,
            building.NeedWorkload,
            userName: (building.TheUsingPawn != null) ? building.TheUsingPawn.name : null
            );
    }

    protected override void AddPanel()
    {
        PanelManager.Instance.RemovePanel(PanelManager.Instance.GetTopPanel());
        PanelManager.Instance.AddPanel(new DetailViewPanel(building, StartShow, EndShow));
    }
}