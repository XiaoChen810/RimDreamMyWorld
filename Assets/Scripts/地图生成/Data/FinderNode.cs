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
        public const float s_MaxIntoCost = 99999;
        public int cols, rows;
        public float Fcost, Gcost, Hcost;
        public float intoCost;
        public bool walkable
        {
            get
            {
                return intoCost < s_MaxIntoCost;
            }
        }
        public FinderNode father;

        public FinderNode(int x, int y, float intoCost)
        {
            this.cols = x;
            this.rows = y;
            this.intoCost = intoCost;
        }
    }
}
