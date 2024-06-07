using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChenChen_Thing
{
    public class Thing_Demolish : ThingBase
    {
        public override void OnPlaced()
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(transform.position);

            // 遍历检测到的建筑结构
            foreach (var coll in colliders)
            {
                if(coll.TryGetComponent<Thing_Architectural>(out var architectural))
                {
                    architectural.Demolish();
                    break;
                }
            }
            Destroy(gameObject);
        }

        public override bool CanBuildHere()
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(transform.position);

            if (colliders.Any(x => x.TryGetComponent<Thing_Architectural>(out var _))) return true;

            return false;
        }
    }
}