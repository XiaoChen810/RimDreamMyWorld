using ChenChen_BuildingSystem;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UISystem
{
    public class DetailViewPanel_Thing : DetailViewPanel
    {
        private ThingBase thing;

        public DetailViewPanel_Thing(ThingBase thing, Callback onEnter, Callback onExit) : base(onEnter, onExit) 
        {
            this.thing = thing;
        }    
    }
}