using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_MapGenerator
{
    /// <summary>
    ///  Ѱ·�㷨�Ľڵ㣬��ʼ��ʱҪ����<see langword="bool"/> walkable �ж��Ƿ����ͨ��
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
