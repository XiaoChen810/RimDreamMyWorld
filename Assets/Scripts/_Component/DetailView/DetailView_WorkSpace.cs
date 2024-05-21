﻿using ChenChen_UISystem;
using UnityEngine;
using System;
using System.Collections.Generic;


public class DetailView_WorkSpace : DetailView
{
    [SerializeField] protected WorkSpace workSpace;

    private void OnEnable()
    {
        workSpace = GetComponent<WorkSpace>();
    }

    protected override void AddPanel()
    {
        PanelManager panel = DetailViewManager.Instance.PanelManager;
        panel.RemoveTopPanel(panel.GetTopPanel());
        panel.AddPanel(new DetailViewPanel_WorkSpace(workSpace, StartShow, EndShow));
    }

    protected override void UpdateShow(DetailViewPanel panel)
    {
        if (panel == null) return;
        if (workSpace == null) return;
        Content.Clear();
        // 如果工作区是农业类型的特殊情况
        if(TryGetComponent<WorkSpace_Farm>(out WorkSpace_Farm workSpace_Farm))
        {
            Content.Add($"当前种植作物: {workSpace_Farm.CurCrop.CropName}");
        }
        //Content.Add($"使用者: {(workSpace.TheUsingPawn != null ? workSpace.TheUsingPawn.name : null)}");
        panel.SetView(
            workSpace.name,
            Content
            );
    }
}

