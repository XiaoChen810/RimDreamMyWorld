using ChenChen_UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using ChenChen_Core;

namespace ChenChen_Thing
{
    public class Thing_SewingTable : Thing_Tool
    {
        private ApparelDef currentApparelDef = null;

        public ApparelDef CurrentApparelDef
        {
            get
            {
                return currentApparelDef;
            }
            set
            {
                Debug.Log("设置了制作的衣物");
                currentApparelDef = value;
            }
        }

        public bool isOpenMenu;

        public override void OpenMenu()
        {
            if (!isOpenMenu)
            {
                isOpenMenu = true;
                PanelManager.Instance.AddPanel(new Panel_SewingTable(this));
            }
            DetailView.ClosePanel();
        }
    }
}
