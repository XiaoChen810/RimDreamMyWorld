﻿using ChenChen_BuildingSystem;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UISystem
{
    public class DetailViewPanel_WorkSpace : DetailViewPanel
    {
        private WorkSpace workSpace;

        public DetailViewPanel_WorkSpace(WorkSpace workSpace, Callback onEnter, Callback onExit) : base(onEnter, onExit)
        {
            this.workSpace = workSpace;
        }

        public override void OnEnter()
        {
            base.OnEnter();


        }
    }
}