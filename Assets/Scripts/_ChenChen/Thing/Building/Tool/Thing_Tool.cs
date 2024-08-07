using ChenChen_Core;
using ChenChen_UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Thing
{
    public class Thing_Tool : Building
    {        
        /// <summary>
        /// 制造所需材料
        /// </summary>
        public virtual IReadOnlyList<(string, int)> RequiredForMade { get; }

        public virtual bool IsFullRequiredForMade { get { return true; } }

        public virtual void OpenWorkMenu()
        {
            Debug.Log("未设置");
        }

        protected override void DetailViewOverrideContentAction(DetailViewPanel panel)
        {
            if (panel == null) return;

            List<String> content = new List<String>();

            InitDetailViewContent(content);

            if (!this.IsFullRequiredForMade)
            {
                foreach (var it in RequiredForMade)
                {
                    content.Add($"制作需要{XmlLoader.Instance.GetDef(it.Item1).name}: {it.Item2}");
                }
            }

            InitDetailViewButton(panel);

            if (this.LifeState == BuildingLifeStateType.FinishedBuilding)
            {
                panel.SetButton("菜单", () =>
                {
                    this.OpenWorkMenu();
                });
            }

            panel.SetView(this.Def.DefName, content);
        }
    }
}
