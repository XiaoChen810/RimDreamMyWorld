using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChenChen_Core;
using ChenChen_UI;

namespace ChenChen_Thing
{
    public class Item : Thing
    {
        public Def Def;
        private int num;

        public string Label => Def.label;
        public int Num
        {
            get
            {
                return num;
            }
            set
            {
                num = value;
                if (num <= 0)
                {
                    Destroy(this.gameObject);
                }
            }
        }

        public void Init(Def def, int num)
        {
            Def = def;
            Num = num;
        }

        protected virtual void DetailViewOverrideContentAction(DetailViewPanel panel)
        {
            if (panel == null) return;
            List<String> content = new List<String>();

            content.Add($"耐久度: {this.Durability} / {this.MaxDurability}");
            content.Add($"使用者: {(this.UserPawn != null ? this.UserPawn.name : null)}");
            content.Add($"数量：{this.Num}");

            panel.SetView(this.Def.name, content);
        }

        protected override void Start()
        {
            base.Start();

            DetailView.OverrideContentAction = DetailViewOverrideContentAction;
        }
    }
}
