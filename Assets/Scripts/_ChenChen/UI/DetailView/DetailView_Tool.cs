using ChenChen_Thing;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_UI
{
    public class DetailView_Tool : DetailView
    {
        [SerializeField] protected Thing_Tool tool;

        private void OnEnable()
        {
            tool = GetComponent<Thing_Tool>();
        }

        public override void OpenPanel()
        {
            PanelManager panelManager = DetailViewManager.Instance.PanelManager;
            panelManager.RemovePanel(panelManager.GetTopPanel());
            panelManager.AddPanel(new DetailViewPanel_Tool(tool, StartShow, EndShow));
        }


        protected override void UpdateShow(DetailViewPanel panel)
        {

            if (panel == null) return;
            if (tool == null) return;
            content.Clear();
            content.Add($"耐久度: {tool.CurDurability} / {tool.MaxDurability}");
            content.Add($"剩余工作量: {tool.Workload}");
            content.Add($"使用者: {(tool.TheUsingPawn != null ? tool.TheUsingPawn.name : null)}");
            panel.SetView(
            tool.Def.DefName,
                content
            );

            panel.RemoveAllButton();

            if (tool.LifeState == BuildingLifeStateType.MarkBuilding)
            {
                panel.SetButton("取消", () =>
                {
                    tool.OnCancelBuild();
                });
            }
            if (tool.LifeState == BuildingLifeStateType.MarkDemolished)
            {
                panel.SetButton("取消", () =>
                {
                    tool.OnCanclDemolish();
                });
            }
            if (tool.LifeState == BuildingLifeStateType.FinishedBuilding)
            {
                panel.SetButton("拆除", () =>
                {
                    tool.MarkToDemolish();
                });
            }

            panel.SetButton("菜单", () =>
            {
                tool.OpenMenu();
            });
        }
    }
}
