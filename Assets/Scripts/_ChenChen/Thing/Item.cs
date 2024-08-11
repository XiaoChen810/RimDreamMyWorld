using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChenChen_Core;
using ChenChen_UI;
using ChenChen_AI;

namespace ChenChen_Thing
{
    public class Item : Thing
    {
        public Def Def;    

        public string Label => Def.label;

        [SerializeField] private int num;
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

        public bool IsOnStorage;

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

        public override string CommandName => "优先搬运";

        public override void CommandFunc(Pawn p)
        {
            Debug.Log("3");
        }
    }
}
