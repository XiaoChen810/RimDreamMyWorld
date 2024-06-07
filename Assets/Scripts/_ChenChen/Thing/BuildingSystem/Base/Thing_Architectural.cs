using ChenChen_Map;
using UnityEngine.Tilemaps;
using UnityEngine;

namespace ChenChen_Thing
{
    public class Thing_Architectural : ThingBase
    {
        public override void OnPlaced()
        {
            // 设置初始值
            Workload = WorkloadBuilt;
            CurDurability = MaxDurability;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }
    }
}