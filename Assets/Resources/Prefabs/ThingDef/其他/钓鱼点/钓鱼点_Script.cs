using ChenChen_MapGenerator;
using System.Linq;
using UnityEngine;

namespace ChenChen_BuildingSystem
{
    public class 钓鱼点_Script : Thing_Building
    {
        public Collider2D fishWhere;

        public override bool CanBuildHere()
        {
            // 获取待放置对象的 Collider2D 组件
            Collider2D collider = ColliderSelf;
            // 获取待放置对象 Collider2D 的边界框信息
            Bounds bounds1 = collider.bounds;
            // 计算碰撞体在世界空间中的中心位置
            Vector2 center = bounds1.center;
            // 执行碰撞检测
            Collider2D[] colliders = Physics2D.OverlapBoxAll(center, bounds1.size, 0f);

            // 如果有其他碰撞体，返回false
            if (colliders.Any(otherCollider => otherCollider.gameObject != this.gameObject))
            {
                return false;
            }

            // 检查鱼点区域是否为水域
            Bounds bounds2 = fishWhere.bounds;
            for (float i = bounds2.min.x; i < bounds2.max.x; i += 0.1f) // 增加检测精度
            {
                for (float j = bounds2.min.y; j < bounds2.max.y; j += 0.1f)
                {
                    if (MapManager.Instance.GetMapNodeHere(new Vector2(i, j)).type != NodeType.water)
                    {
                        return false;
                    }
                }
            }

            // 有水，且没有其他碰撞体的情况返回true
            return true;
        }
    }
}
