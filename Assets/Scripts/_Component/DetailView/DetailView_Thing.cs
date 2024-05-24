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
    public override void OpenPanel()
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
        Content.Add($"�;ö�: {thing.CurDurability} / {thing.MaxDurability}");
        Content.Add($"ʣ�๤����: {thing.Workload}");
        Content.Add($"ʹ����: {(thing.TheUsingPawn != null ? thing.TheUsingPawn.name : null)}");
        panel.SetView(
            thing.Def.DefName,
            Content
            );

        if (thing.LifeState == BuildingLifeStateType.MarkBuilding)
        {
            panel.RemoveAllButton();
            panel.SetButton("ȡ��", () =>
            {
                thing.OnCancelBuild();
            });
        }
        if (thing.LifeState == BuildingLifeStateType.MarkDemolished)
        {
            panel.RemoveAllButton();
            panel.SetButton("ȡ��", () =>
            {
                thing.OnCanclDemolish();
            });
        }
        if (thing.LifeState == BuildingLifeStateType.FinishedBuilding)
        {
            panel.RemoveAllButton();
            panel.SetButton("���", () =>
            {
                thing.ChangeLifeState(BuildingLifeStateType.MarkDemolished);
            });
        }
    }

    public override void StartShow()
    {
        base.StartShow();
    }

    public override void EndShow()
    {
        base.EndShow();
    }
}
