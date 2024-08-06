using ChenChen_UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Thing
{
    public class Thing_Tool : Building
    {
        public bool IsSuccess { get; private set; }

        public TargetPtr wookPos;

        public virtual void OpenMenu()
        {
            Debug.Log("未设置");
        }

        protected override Action<DetailViewPanel> DetailViewOverrideContentAction()
        {
            return (DetailViewPanel panel) =>
            {
                List<String> content = new List<String>();
                if (panel == null) return;
                content.Clear();
                content.Add($"耐久度: {this.Durability} / {this.MaxDurability}");
                content.Add($"剩余工作量: {this.Workload}");
                content.Add($"使用者: {(this.UserPawn != null ? this.UserPawn.name : null)}");
                panel.SetView(
                this.Def.DefName,
                    content
                );

                panel.RemoveAllButton();

                if (this.LifeState == BuildingLifeStateType.MarkBuilding)
                {
                    panel.SetButton("取消", () =>
                    {
                        this.OnCancelBuild();
                    });
                }
                if (this.LifeState == BuildingLifeStateType.MarkDemolished)
                {
                    panel.SetButton("取消", () =>
                    {
                        this.OnCanclDemolish();
                    });
                }
                if (this.LifeState == BuildingLifeStateType.FinishedBuilding)
                {
                    panel.SetButton("拆除", () =>
                    {
                        this.MarkToDemolish();
                    });
                }
                if (this.LifeState == BuildingLifeStateType.FinishedBuilding)
                {
                    panel.SetButton("菜单", () =>
                    {
                        this.OpenMenu();
                    });
                }
            };
        }
    }
}
