using ChenChen_UISystem;
using UnityEngine;
using System;
using System.Collections.Generic;
using ChenChen_AI;

public class DetailView_Pawn : DetailView
{
    [SerializeField] protected Pawn pawn;

    private void OnEnable()
    {
        pawn = GetComponent<Pawn>();
    }

    public override void OpenIndicator()
    {
        if (indicator == null)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("Views/Indicator"), gameObject.transform);
            indicator = go.GetComponent<Indicator>();
            indicator.gameObject.name = "Indicator";
            go.transform.position += new Vector3(0, 0.4f, 0);
        }
        IsIndicatorOpen = true;
        indicator.gameObject.SetActive(true);
        pawn.Info.IsSelect = true;
    }

    public override void CloseIndicator()
    {
        if (indicator == null)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("Views/Indicator"), gameObject.transform);
            indicator = go.GetComponent<Indicator>();
            indicator.gameObject.name = "Indicator";
            go.transform.position += new Vector3(0, 0.4f, 0);
        }
        IsIndicatorOpen = false;
        indicator.gameObject.SetActive(false);
        pawn.Info.IsSelect = false;
    }

    public override void OpenPanel()
    {
        PanelManager panel = DetailViewManager.Instance.PanelManager;
        panel.RemoveTopPanel(panel.GetTopPanel());
        panel.AddPanel(new DetailViewPanel_Pawn(pawn, StartShow, EndShow));
    }

    protected override void UpdateShow(DetailViewPanel panel)
    {
        if (panel == null) return;
        if (pawn == null) return;
        Content.Clear();
        Content.AddRange(pawn.CurrentStateList);
        panel.SetView(
            pawn.name,
            Content
            );
    }
}
