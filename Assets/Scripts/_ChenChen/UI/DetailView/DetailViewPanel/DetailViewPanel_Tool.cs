using ChenChen_Thing;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_UI
{
    public class DetailViewPanel_Tool : DetailViewPanel
    {
        private Thing_Tool tool;

        public DetailViewPanel_Tool(Thing_Tool tool, Callback onEnter, Callback onExit) : base(onEnter, onExit)
        {
            this.tool = tool;
        }
    }
}
