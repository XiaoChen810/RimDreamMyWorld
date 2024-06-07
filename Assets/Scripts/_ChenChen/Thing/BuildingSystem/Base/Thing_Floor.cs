using ChenChen_Map;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using static ChenChen_Thing.WorkSpace_Farm;

namespace ChenChen_Thing
{
    public class Thing_Floor : Thing_Architectural  
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            tag = "Floor";
            GetComponent<SpriteRenderer>().sortingLayerName = "Bottom";
            GetComponent<SpriteRenderer>().sortingOrder = -1;
        }
    }
}
