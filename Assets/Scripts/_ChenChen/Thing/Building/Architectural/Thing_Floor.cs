using ChenChen_Map;
using System;
using UnityEngine;

namespace ChenChen_Thing
{
    public class Thing_Floor : Thing_Architectural  
    {
        protected override void Start()
        {
            base.Start();
            SR.sortingLayerName = "Middle";
        }
    }
}
