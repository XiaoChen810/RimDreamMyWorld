using ChenChen_UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Thing
{
    public class Thing_SewingTable : Thing_Tool
    {
        private AppealDef currentAppealDef = null;

        public AppealDef CurrentAppealDef
        {
            get
            {
                return currentAppealDef;
            }
            set
            {
                Debug.Log("设置了制作的衣物");
                currentAppealDef = value;
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
