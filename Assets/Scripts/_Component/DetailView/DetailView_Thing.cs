using ChenChen_BuildingSystem;
using ChenChen_UISystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailView_Thing : DetailView
{
    [SerializeField] protected ThingBase thing;

    private void OnEnable()
    {
        thing = GetComponent<ThingBase>();
    }
    protected override void AddPanel()
    {
        PanelManager panelManager = DetailViewManager.Instance.PanelManager;
        panelManager.RemoveTopPanel(panelManager.GetTopPanel());
        panelManager.AddPanel(new DetailViewPanel_Thing(thing, StartShow, EndShow));
    }

    protected override void UpdateShow(DetailViewPanel panel)
    {
        if (panel == null) return;
        if (thing == null) return;
        Content.Clear();
        Content.Add($"耐久度: {thing.CurDurability} / {thing.MaxDurability}");
        Content.Add($"剩余工作量: {thing.Workload}");
        Content.Add($"使用者: {(thing.TheUsingPawn != null ? thing.TheUsingPawn.name : null)}");
        panel.SetView(
            thing.Def.DefName,
            Content
            );
    }

    public override void StartShow()
    {
        base.StartShow();
        GetComponent<ThingBase>().DrawOutline_Sprite = true;
    }

    public override void EndShow()
    {
        base.EndShow();
        GetComponent<ThingBase>().DrawOutline_Sprite = false;
    }
}
