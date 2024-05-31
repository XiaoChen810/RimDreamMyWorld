using ChenChen_Map;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

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

            // 遍历检测到的碰撞器，如果有任何一个碰撞器存在，则返回 false，表示无法放置游戏对象
            foreach (Collider2D otherCollider in colliders)
            {
                if (otherCollider.gameObject != this.gameObject) // 忽略自己游戏对象的碰撞
                {
                    // 只要没有放过地板或者墙壁都可以放置在这里
                    if(otherCollider.gameObject.TryGetComponent<ThingBase>(out ThingBase thing))
                    {
                        if (thing is Thing_Floor)
                        {
                            return false;
                        }
                        if (thing is Thing_Wall)
                        {
                            return false;
                        }
                    }
                }
            }

            // 如果没有任何碰撞器存在，则表示可以放置游戏对象
            return true;
        }
    }
}
