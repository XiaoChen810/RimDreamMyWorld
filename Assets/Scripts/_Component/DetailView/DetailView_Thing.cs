using ChenChen_BuildingSystem;
using ChenChen_UISystem;
using UnityEngine;
using UnityEngine.UI;

public class DetailView_Thing : DetailView
{
    protected ThingBase thing;

    private void OnEnable()
    {
        thing = GetComponent<ThingBase>();
    }

    protected override void OnMouseDown()
    {
        if (BuildingSystemManager.Instance.Tool.OnBuildMode) return;
        base.OnMouseDown();
    }

    protected override void UpdateShow(DetailViewPanel panel)
    {
        if (panel == null) return;
        if (thing == null) return;

        panel.SetView(
            thing.Def.DefName,
            thing.MaxDurability,
            thing.CurDurability,
            thing.Workload,
            userName: (thing.TheUsingPawn != null) ? thing.TheUsingPawn.name : null
            );
    }

    protected override void AddPanel()
    {
        PanelManager.Instance.RemovePanel(PanelManager.Instance.GetTopPanel());
        PanelManager.Instance.AddPanel(new DetailViewPanel(thing, StartShow, EndShow));
    }
}