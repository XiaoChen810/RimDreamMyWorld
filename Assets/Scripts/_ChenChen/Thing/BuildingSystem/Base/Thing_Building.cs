using ChenChen_Map;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ChenChen_Thing
{
    public class Thing_Building : ThingBase
    {
        public override void OnPlaced()
        {
            // ���ó�ʼֵ
            Workload = WorkloadBuilt;
            CurDurability = MaxDurability;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }
    }
}