using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_MapGenerator
{
    /// <summary>
    ///  寻路算法的节点，初始化时要给出<see langword="bool"/> walkable 判断是否可以通行
    /// </summary>
    public class FinderNode
    {
        public int cols, rows;
        public float Fcost, Gcost, Hcost;
        public FinderNode father;
        public bool walkable;

        public FinderNode(int x, int y, bool walkable)
        {
            this.cols = x;
            this.rows = y;
            this.walkable = walkable;
        }
    }
}
