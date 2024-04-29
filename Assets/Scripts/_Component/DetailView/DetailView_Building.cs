using ChenChen_BuildingSystem;
using ChenChen_UISystem;
using UnityEngine;
using UnityEngine.UI;

public class DetailView_Building : DetailView
{
    protected Thing_Building building;

    private void OnEnable()
    {
        building = GetComponent<Thing_Building>();
    }

    protected override void OnMouseDown()
    {
        if (BuildingSystemManager.Instance.Tool.OnBuildMode) return;
        base.OnMouseDown();
    }

    protected override void UpdateShow(DetailViewPanel panel)
    {
        if (panel == null) return;
        if (building == null) return;

        panel.SetView(
            building.Def.Name,
            building.MaxDurability,
            building.CurDurability,
            building.Workload,
            userName: (building.TheUsingPawn != null) ? building.TheUsingPawn.name : null
            );
    }

    protected override void AddPanel()
    {
        PanelManager.Instance.RemovePanel(PanelManager.Instance.GetTopPanel());
        PanelManager.Instance.AddPanel(new DetailViewPanel(building, StartShow, EndShow));
    }
}
