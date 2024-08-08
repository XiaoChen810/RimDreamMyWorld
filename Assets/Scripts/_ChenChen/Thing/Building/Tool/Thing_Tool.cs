using ChenChen_Core;
using ChenChen_UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Thing
{
    public class Thing_Tool : Building
    {
        [SerializeField] protected Transform workPosition;                // 工作地点
        [SerializeField] protected Transform producePositon;              // 产出地点
        [SerializeField] private int numberOfWaitingToMade = 0;
        [SerializeField] private StuffDef currentMadeDef = null;

        public int NumberOfWaitingToMade
        {
            get
            {
                return numberOfWaitingToMade;
            }
            set
            {
                numberOfWaitingToMade = Mathf.Clamp(value, 0, 99);
            }
        }

        public StuffDef CurrentMadeDef
        {
            get
            {
                return currentMadeDef;
            }
            set
            {
                if (value != null && !value.canMade)
                {
                    throw new Exception("该物品无法被制作");
                }
                else
                {
                    currentMadeDef = value;
                }              
            }
        }
        /// <summary>
        /// 制造所需材料
        /// </summary>
        public virtual IReadOnlyList<(string, int)> RequiredForMade => throw new NotImplementedException();

        public virtual bool IsFullRequiredForMade => throw new NotImplementedException();

        public virtual Sprite CurrentWorkSprite => throw new NotImplementedException();

        public virtual List<StuffDef> AllCanDo => throw new NotImplementedException(); 

        public virtual void OpenWorkMenu()
        {
            Debug.Log("未设置");
        }

        public virtual void CancelCurrentWork()
        {
            CurrentMadeDef = null;
            NumberOfWaitingToMade = 0;
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
