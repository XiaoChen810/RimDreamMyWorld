using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Map
{
    public class PathFinder
    {
        public PathFinder(MapManager mapManager)
        {
            this.mapManager = mapManager;
            InitNode();
        }

        private MapManager mapManager;

        private PathNode[,] origin;
        private PathNode[,] pathNodes;

        private bool hasInit = false;
        private bool useAstar = true; 

        public List<Vector2> seekPath(Vector2 start, Vector2 target)
        {
            if (!hasInit)
            {
                InitNode();
            }
            pathNodes = DeepCopyNodes(origin);
            return useAstar ? Astar(start, target) : JPS(start,target);
        }

        private void InitNode()
        {
            origin = new PathNode[mapManager.CurMapWidth, mapManager.CurMapHeight];
            foreach (var mapNode in mapManager.CurMapNodes)
            {
                origin[mapNode.position.x, mapNode.position.y] = new PathNode(mapNode);
            }
        }

        private PathNode[,] DeepCopyNodes(PathNode[,] original)
        {
            int width = original.GetLength(0);
            int height = original.GetLength(1);
            PathNode[,] copy = new PathNode[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (original[x, y] != null)
                    {
                        copy[x, y] = original[x, y].Clone();
                    }
                }
            }

            return copy;
        }


        #region - Astar - 

        private List<Vector2> Astar(Vector2 start, Vector2 target)
        {
            PathNode begin = TransPosToNode(start);
            PathNode end = TransPosToNode(target);

            if (begin == null || end == null)
            {
                return null;
            }

            List<Vector2> dir = new List<Vector2>
            {
                new Vector2(0,1),
                new Vector2(1,1),
                new Vector2(1,0),
                new Vector2(1,-1),
                new Vector2(0,-1),
                new Vector2(-1,-1),
                new Vector2(-1,0),
                new Vector2(-1,1),
            };
           
            PriorityQueue openSet = new PriorityQueue();
            HashSet<PathNode> closeSet = new HashSet<PathNode>();

            begin.g = 0;
            begin.h = Heuristic(begin, end);
            begin.f = begin.g + begin.h;
            openSet.Add(begin);

            while (openSet.PeekFirst() != null)
            {
                PathNode current = openSet.PopFirst();

                closeSet.Add(current);

                if (current.Equals(end))
                {
                    return ReconstructPath(current);
                }
                
                List<PathNode> neighbors = GetNeighbors(current);

                foreach (PathNode neighbor in neighbors)
                {
                    if (closeSet.Contains(neighbor))
                    {
                        continue;
                    }

                    float gScore = current.g + 1;

                    if ( gScore < neighbor.g)
                    {
                        neighbor.parent = current;
                        neighbor.g = gScore;
                        neighbor.h = Heuristic(neighbor, end);
                        neighbor.f = neighbor.g + neighbor.h;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

            return null;
        }

        #endregion

        #region - JPS - 

        private List<Vector2> JPS(Vector2 start, Vector2 target)
        {
            PathNode begin = TransPosToNode(start);
            PathNode end = TransPosToNode(target);

            if (begin == null || end == null)
            {
                return null;
            }

            HashSet<PathNode> visited = new HashSet<PathNode>();
            PriorityQueue openSet = new PriorityQueue();

            // 起点是跳点，加入openSet
            begin.g = 0;
            begin.h = Heuristic(begin, end);
            begin.f = begin.g + begin.h;
            openSet.Add(begin);

            while (openSet.PeekFirst() != null)
            {
                PathNode current = openSet.PopFirst();
                visited.Add(current);

                if (current == end)
                {
                    List<Vector2> path = ReconstructPath(current);
                    return path;
                }

                // 判断继任节点
                IdentifySuccessors(current, end, visited, openSet);
            }

            return null;
        }

        private void IdentifySuccessors(PathNode node, PathNode end, HashSet<PathNode> visited, PriorityQueue openSet)
        {
            // 获取邻居节点
            List<PathNode> neighbors = GetNeighbors(node);

            foreach (PathNode neighbor in neighbors)
            {
                Vector2 direction = neighbor.position - node.position;
                PathNode jumpNode = Jump(node, direction, end);

                if (jumpNode != null && !visited.Contains(jumpNode))
                {
                    if (jumpNode.g > node.g + Heuristic(node, jumpNode))
                    {
                        jumpNode.g = node.g + Heuristic(node, jumpNode);
                        jumpNode.h = Heuristic(jumpNode, end);
                        jumpNode.f = jumpNode.g + jumpNode.h;
                        jumpNode.parent = node;

                        openSet.Add(jumpNode);
                    }
                }
            }
        }

        private PathNode Jump(PathNode current, Vector2 direction, PathNode end)
        {
            Vector2 nextPos = current.position + direction;
            PathNode nextNode = TransPosToNode(nextPos);

            // 撞墙，或到地图边界视为寻找失败
            if (nextNode == null || IsBlocked(nextNode))
            {
                return null;
            }

            // 目标点是跳点
            if (nextNode == end )
            {
                return nextNode;
            }

            // 如果有强迫邻居，则视为跳点
            if (HasForcedNeighbors(nextNode, direction))
            {
                return nextNode;
            }

            // 如果是对角线移动，并且经过水平或垂直方向移动可以到达跳点，则视为跳点
            if (direction.x != 0 && direction.y != 0)
            {
                if (Jump(nextNode, new Vector2(direction.x, 0), end) != null ||
                    Jump(nextNode, new Vector2(0, direction.y), end) != null)
                {
                    return nextNode;
                }
            }

            // 没找到则继续按照方向寻找
            return Jump(nextNode, direction, end);
        }

        private bool IsBlocked(PathNode node)
        {
            if (node == null)
            {
                return true;
            }
            if (node.MapNode.type == NodeType.water)
            {
                return true;
            }
            return false;
        }

        private bool HasForcedNeighbors(PathNode node, Vector2 direction)
        {
            // 判断当前节点是否有强制邻居
            Vector2 pos = node.position;

            // 根据方向判断
            DIR dir = CheckDIR(direction);

            if (dir == DIR.RIGHT)   // 右
            {
                return (IsBlocked(TransPosToNode(pos + new Vector2(-1, 1))) && !IsBlocked(TransPosToNode(pos + new Vector2(0, 1)))) ||
                       (IsBlocked(TransPosToNode(pos + new Vector2(-1, -1))) && !IsBlocked(TransPosToNode(pos + new Vector2(0, -1))));
            }
            if (dir == DIR.LEFT)    // 左
            {
                return (IsBlocked(TransPosToNode(pos + new Vector2(1, 1))) && !IsBlocked(TransPosToNode(pos + new Vector2(0, 1)))) ||
                       (IsBlocked(TransPosToNode(pos + new Vector2(1, -1))) && !IsBlocked(TransPosToNode(pos + new Vector2(0, -1))));

            }
            if (dir == DIR.UP)      // 上
            {
                return (IsBlocked(TransPosToNode(pos + new Vector2(1, -1))) && !IsBlocked(TransPosToNode(pos + new Vector2(1, 0)))) ||
                           (IsBlocked(TransPosToNode(pos + new Vector2(-1, -1))) && !IsBlocked(TransPosToNode(pos + new Vector2(-1, 0))));
            }
            if (dir == DIR.DOWN)    // 下
            {
                return (IsBlocked(TransPosToNode(pos + new Vector2(1, 1))) && !IsBlocked(TransPosToNode(pos + new Vector2(1, 0)))) ||
                           (IsBlocked(TransPosToNode(pos + new Vector2(-1, 1))) && !IsBlocked(TransPosToNode(pos + new Vector2(-1, 0))));
            }
            if (dir == DIR.RIGHT_UP)    // 右上
            {
                return IsBlocked(TransPosToNode(pos + new Vector2(-1, 0))) || IsBlocked(TransPosToNode(pos + new Vector2(0, -1)));
            }
            if (dir == DIR.RIGHT_DOWN)  // 右下
            {
                return IsBlocked(TransPosToNode(pos + new Vector2(-1, 0))) || IsBlocked(TransPosToNode(pos + new Vector2(0, 1)));
            }
            if (dir == DIR.LEFT_UP)     // 左上
            {
                return IsBlocked(TransPosToNode(pos + new Vector2(1, 0))) || IsBlocked(TransPosToNode(pos + new Vector2(0, -1)));
            }
            if (dir == DIR.LEFT_DOWN)   // 左下
            {
                return IsBlocked(TransPosToNode(pos + new Vector2(1, 0))) || IsBlocked(TransPosToNode(pos + new Vector2(0, 1)));
            }

            return false;
        }

        private enum DIR { RIGHT, LEFT, UP, DOWN, RIGHT_UP, RIGHT_DOWN, LEFT_UP, LEFT_DOWN ,ZERO }

        private DIR CheckDIR(Vector2 direction)
        {
            if (direction.x == 1 && direction.y == 0)
            {
                return DIR.RIGHT;
            }
            else if (direction.x == -1 && direction.y == 0)
            {
                return DIR.LEFT;
            }
            else if (direction.x == 0 && direction.y == 1)
            {
                return DIR.UP;
            }
            else if (direction.x == 0 && direction.y == -1)
            {
                return DIR.DOWN;
            }
            else if (direction.x == 1 && direction.y == 1)
            {
                return DIR.RIGHT_UP;
            }
            else if (direction.x == 1 && direction.y == -1)
            {
                return DIR.RIGHT_DOWN;
            }
            else if (direction.x == -1 && direction.y == 1)
            {
                return DIR.LEFT_UP;
            }
            else if (direction.x == -1 && direction.y == -1)
            {
                return DIR.LEFT_DOWN;
            }
            else
            {
                return DIR.ZERO;
            }
        }

        #endregion

        #region - 通用 -

        // 将坐标转换成节点
        private PathNode TransPosToNode(Vector2 pos)
        {
            if (pos.x >= 0 && pos.x < mapManager.CurMapWidth && pos.y >= 0 && pos.y < mapManager.CurMapHeight)
            {
                return pathNodes[(int)pos.x, (int)pos.y];
            }
            return null;
        }

        private List<PathNode> GetNeighbors(PathNode current)
        {
            // 返回四方向
            //List<Vector2> dir = new List<Vector2>
            //{
            //    new Vector2(0,1),
            //    new Vector2(1,0),
            //    new Vector2(0,-1),
            //    new Vector2(-1,0),
            //};
            //List<PathNode> neighbors = new List<PathNode>();
            //foreach (var d in dir)
            //{
            //    var node = TransPosToNode(current.position + d);
            //    if (node.IsWalkable)
            //    {
            //        neighbors.Add(node);
            //    }
            //}

            // 返回八方向
            List<Vector2> dir = new List<Vector2>
            {
                new Vector2(0,1),
                new Vector2(1,1),
                new Vector2(1,0),
                new Vector2(1,-1),
                new Vector2(0,-1),
                new Vector2(-1,-1),
                new Vector2(-1,0),
                new Vector2(-1,1),
            };
            List<PathNode> neighbors = new List<PathNode>();
            foreach (var d in dir)
            {
                var node = TransPosToNode(current.position + d);
                if (node.IsWalkable)
                {
                    neighbors.Add(node);
                }
            }

            return neighbors;
        }

        // 启发函数
        private float Heuristic(PathNode a, PathNode b)
        {
            return Vector2.Distance(a.position, b.position);
        }

        // 找到目标点后反向构建路径
        private List<Vector2> ReconstructPath(PathNode current)
        {
            List<Vector2> path = new List<Vector2>();
            while (current != null)
            {
                path.Add(current.position);
                current = current.parent;
            }
            path.Reverse();
            return path;
        }

        #endregion
    }

    #region - 数据结构 -

    // 用于寻路的节点
    public class PathNode
    {
        public MapNode MapNode { get; private set; }
        public Vector2 position => MapNode.position;
        public bool IsWalkable => MapNode.type != NodeType.water;

        public float f = float.MaxValue;
        public float g = float.MaxValue;
        public float h = 0;
        public PathNode parent;

        public PathNode(MapNode mapNode)
        {
            MapNode = mapNode;
        }

        public PathNode Clone()
        {
            PathNode clone = (PathNode)MemberwiseClone();
            clone.f = float.MaxValue;
            clone.g = float.MaxValue;
            clone.h = 0;
            clone.parent = null;
            return clone;
        }
    }

    // 依照路径代价判断的小根堆
    public class PriorityQueue
    {
        private List<PathNode> heap = new List<PathNode>();

        private int Farther(int i)
        {
            return (i - 1) / 2;
        }
        private int LeftChild(int i)
        {
            return 2 * i + 1;
        }
        private int RightChild(int i)
        {
            return 2 * i + 2;
        }

        public void Add(PathNode node)
        {
            heap.Add(node);
            int i = heap.Count - 1;
            while (i != 0 && heap[i].f < heap[Farther(i)].f)
            {
                var temp = heap[i];
                heap[i] = heap[Farther(i)];
                heap[Farther(i)] = temp;

                i = Farther(i);
            }
        }

        public PathNode PopFirst()
        {
            if (heap.Count == 0)
            {
                return null;
            }

            PathNode res = heap[0];
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            int i = 0;
            while (true)
            {
                int left = LeftChild(i);
                int right = RightChild(i);
                int smallest = i;

                if (left < heap.Count && heap[left].f < heap[smallest].f)
                {
                    smallest = left;
                }
                if (right < heap.Count && heap[right].f < heap[smallest].f)
                {
                    smallest = right;
                }
                if (smallest == i)
                {
                    break;
                }

                var temp = heap[i];
                heap[i] = heap[smallest];
                heap[smallest] = temp;

                i = smallest;
            }

            return res;
        }

        public PathNode PeekFirst()
        {
            if (heap.Count == 0)
            {
                return null;
            }
            return heap[0];
        }

        public bool Contains(PathNode node)
        {
            return heap.Contains(node);
        }
    }

    #endregion
}
