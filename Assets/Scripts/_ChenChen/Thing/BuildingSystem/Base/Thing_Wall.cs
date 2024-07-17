using System;
using UnityEngine;

namespace ChenChen_Thing
{
    [RequireComponent(typeof(Collider2D))]
    public class Thing_Wall : Thing_Architectural
    {
        private void Start()
        {
            this.tag = "Wall";
        }
    }
}
