using ChenChen_BuildingSystem;
using ChenChen_UISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailView_Tree : DetailView
{
    protected Tree tree;

    private void OnEnable()
    {
        tree = GetComponent<Tree>();
    }

    protected override void UpdateShow(DetailViewPanel panel)
    {
        if (panel == null) return;
        if (tree == null) return;

        panel.SetView(
            "Ê÷",
            tree.MaxDurability,
            tree.CurDurability,
            0
            );
    }

    protected override void AddPanel()
    {
        PanelManager.Instance.RemovePanel(PanelManager.Instance.GetTopPanel());
        PanelManager.Instance.AddPanel(new DetailViewPanel(tree, StartShow, EndShow));
    }
}
