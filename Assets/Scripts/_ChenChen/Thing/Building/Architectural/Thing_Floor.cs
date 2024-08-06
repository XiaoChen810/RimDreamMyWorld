using ChenChen_Map;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using static ChenChen_Thing.WorkSpace_Farm;

namespace ChenChen_Thing
{
    public class Thing_Floor : Thing_Architectural  
    {
        protected override void Start()
        {
            base.Start();

            tag = "Floor";
            GetComponent<SpriteRenderer>().sortingLayerName = "Bottom";
            GetComponent<SpriteRenderer>().sortingOrder = -1;
        }

        public override bool CanBuildHere()
        {
            // 获取待放置对象的 Collider2D 组件
            Collider2D collider = ColliderSelf;
            // 获取待放置对象 Collider2D 的边界框信息
            Bounds bounds = collider.bounds;
            // 计算碰撞体在世界空间中的中心位置
            Vector2 center = bounds.center;
            // 执行碰撞检测，只检测指定图层的碰撞器
            Collider2D[] colliders = Physics2D.OverlapBoxAll(center, bounds.size, 0f);

            // 遍历检测到的碰撞器，如果有任何一个不符合条件，则返回 false，表示无法放置游戏对象
            foreach (var coll in colliders)
            {
                // 有墙体不能放置
                if (coll.CompareTag("Wall")) 
                    return false;
                // 有地板不能放置
                if (coll.CompareTag("Floor") && coll.gameObject != this.gameObject) 
                    return false;              
            }

            return true;
        }
    }
}
