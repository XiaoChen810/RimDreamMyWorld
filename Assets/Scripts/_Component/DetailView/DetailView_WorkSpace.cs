using ChenChen_UISystem;
using UnityEngine;
using System;


public class DetailView_WorkSpace : DetailView
{
    [SerializeField] protected WorkSpace workSpace;

    private void OnEnable()
    {
        workSpace = GetComponent<WorkSpace>();
    }

    protected override void AddPanel()
    {
        PanelManager.Instance.RemoveTopPanel(PanelManager.Instance.GetTopPanel());
        PanelManager.Instance.AddPanel(new DetailViewPanel_WorkSpace(workSpace, StartShow, EndShow));
    }

    protected override void UpdateShow(DetailViewPanel panel)
    {
        if (panel == null) return;
        if (workSpace == null) return;

        panel.SetView(
            workSpace.name,
            0,
            0,
            0,
            userName: (workSpace.TheUsingPawn != null) ? workSpace.TheUsingPawn.name : null
            );
    }
}

