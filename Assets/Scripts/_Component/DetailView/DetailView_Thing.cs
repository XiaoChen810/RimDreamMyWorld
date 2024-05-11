using ChenChen_BuildingSystem;
using ChenChen_UISystem;
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
        PanelManager.Instance.RemoveTopPanel(PanelManager.Instance.GetTopPanel());
        PanelManager.Instance.AddPanel(new DetailViewPanel_Thing(thing, StartShow, EndShow));
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
