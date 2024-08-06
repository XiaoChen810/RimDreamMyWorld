using ChenChen_Core;
using System;
using UnityEngine;

namespace ChenChen_Thing
{
    [RequireComponent(typeof(Collider2D))]
    public class Thing_Wall : Thing_Architectural
    {
        protected override void Start()
        {
            base.Start();
            this.tag = "Wall";
        }

        public override void OnCompleteBuild()
        {
            base.OnCompleteBuild();

            RoomManager.Instance.AddWall(transform.position);
        }

        public override void OnDemolished()
        {
            RoomManager.Instance.RemoveWall(transform.position);

            base.OnDemolished();
        }
    }
}
