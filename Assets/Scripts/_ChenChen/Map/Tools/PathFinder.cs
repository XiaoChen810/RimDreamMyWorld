using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Map
{
    public class PathFinder
    {
        private MapManager mapManager;
        private PathNode[,] pathNodes;

        private void InitNode()
        {
            pathNodes = new PathNode[mapManager.CurMapWidth, mapManager.CurMapHeight];
            foreach (var mapNode in mapManager.CurMapNodes)
            {
                pathNodes[mapNode.position.x, mapNode.position.y] = new PathNode(mapNode);
            }
        }

        public List<Vector2> seekPath(Vector2 start, Vector2 target)
        {
            List<Vector2> res = new List<Vector2>();

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

            HashSet<PathNode> visited = new HashSet<PathNode>();
            PriorityQueue openSet = new PriorityQueue();
            begin.g = 0;
            begin.h = Heuristic(begin, end);
            begin.f = begin.g + begin.h;
            openSet.Add(begin);

            while (openSet.PeekFirst() != null)
            {
                PathNode current = openSet.PopFirst();

                if (current.Equals(end))
                {
                    return ReconstructPath(current);
                }

                visited.Add(current);

                foreach (Vector2 direction in dir)
                {
                    Vector2 neighborPos = new Vector2(current.Position.x + direction.x, current.Position.y + direction.y);
                    PathNode neighbor = TransPosToNode(neighborPos);

                    if (neighbor == null || visited.Contains(neighbor) || !neighbor.IsWalkable)
                    {
                        continue;
                    }

                    float gScore = current.g + 1;

                    if (gScore < neighbor.g)
                    {
                        neighbor.Parent = current;
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

        // 将坐标转换成节点
        private PathNode TransPosToNode(Vector2 pos)
        {
            if (pos.x >= 0 && pos.x < mapManager.CurMapWidth && pos.y >= 0 && pos.y < mapManager.CurMapHeight)
            {
                return pathNodes[(int)pos.x, (int)pos.y];
            }
            return null;
        }

        // 启发函数
        private float Heuristic(PathNode a, PathNode b)
        {
            return Vector2.Distance(a.Position, b.Position);
        }

        // 找到目标点后反向构建路径
        private List<Vector2> ReconstructPath(PathNode current)
        {
            List<Vector2> path = new List<Vector2>();
            while (current != null)
            {
                path.Add(current.Position);
                current = current.Parent;
            }
            path.Reverse();
            return path;
        }
    }

    // 用于寻路的节点
    public class PathNode
    {
        public MapNode MapNode { get; private set; }
        public Vector2 Position => MapNode.position;
        public bool IsWalkable => MapNode.type != NodeType.water;

        public float f = float.MaxValue;
        public float g = float.MaxValue;
        public float h = 0;
        public PathNode Parent;

        public PathNode(MapNode mapNode)
        {
            MapNode = mapNode;
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
}
