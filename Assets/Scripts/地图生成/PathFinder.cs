using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace 地图生成
{
    public class PathFinder : MonoBehaviour
    {
        public class Node
        {
            public int x, y;
            public float f, g, h;
            public Node father;
            public bool walkable;

            public Node(int x, int y, bool walkable)
            {
                this.x = x;
                this.y = y;
                this.walkable = walkable;
            }
        }

        private List<Node> openSet = new List<Node>();
        private List<Node> closeSet = new List<Node>();

        Node[,] nodes;
        int mapWidth;
        int mapHeight;

        #region Public

        public Node[,] InitNodes(int width, int height, MapCreator.TileData[,] mapTileData)
        {
            Node[,] nodes = new Node[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    nodes[i, j] = new Node(i, j, !(mapTileData[i, j].type == MapCreator.TileData.Type.water));
                }
            }

            return nodes;
        }

        public List<Vector2> FindPath(Vector3 startPos, Vector3 targetPos, string mapName)
        {
            nodes = MapManager.Instance.sceneMapDatasDict[mapName].nodes;
            mapWidth = MapManager.Instance.sceneMapDatasDict[mapName].width;
            mapHeight = MapManager.Instance.sceneMapDatasDict[mapName].height;

            // 先判断地图有没有初始化
            if (nodes == null)
            {
                Debug.LogWarning($"地图{mapName}还未初始化");
                return null;
            }
            // 从位置转换成节点信息
            Node startNode = TransfromVectorToNode(startPos);
            Node endNode = TransfromVectorToNode(targetPos);

            // 判断是否可行
            if (startNode == null || endNode == null
                || !startNode.walkable || !endNode.walkable)
            {
                Debug.LogWarning($"起点 {startPos} 或终点 {targetPos} 不可达");
                return null;
            }

            // 初始化
            startNode.f = 0;
            startNode.g = 0;
            startNode.h = 0;
            startNode.father = null;

            openSet.Clear();
            closeSet.Clear();

            openSet.Add(startNode);
            while (openSet.Count > 0)
            {
                // 取第一个代价最小的节点
                Node currentNode = openSet[0];

                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].f < currentNode.f || openSet[i].f == currentNode.f && openSet[i].h < currentNode.h)
                    {
                        currentNode = openSet[i];
                    }
                }

                // 将节点移动到close列表 
                openSet.Remove(currentNode);
                closeSet.Add(currentNode);

                // 当找到终点结束
                if (currentNode == endNode)
                {
                    // 结束后计算Close列表，返回路径列表
                    List<Vector2> path = new List<Vector2>();
                    while (endNode.father != null)
                    {
                        path.Add(new Vector2(endNode.x, endNode.y));
                        endNode = endNode.father;
                    }
                    path.Reverse();
                    return path;
                }

                // 处理计算邻居数据
                FindNeighborNodeEight(currentNode, endNode);
            }

            return null;

        }

        #endregion

        #region AStar

        private Node TransfromVectorToNode(Vector2 pos)
        {
            int x, y;
            x = Mathf.FloorToInt(pos.x);
            y = Mathf.FloorToInt(pos.y);
            // 判断范围 
            if (x < 0 || x >= mapWidth) return null;
            if (y < 0 || y >= mapHeight) return null;
            return nodes[x, y];
        }

        private float GetDistance(Node a, Node b)
        {
            float result;
            result = Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
            return result;
        }

        private void FindNeighborNodeEight(Node start, Node end)
        {
            // 八个方向
            int x = start.x;
            int y = start.y;
            PutInOpenSet(x, y + 1, start, end);
            PutInOpenSet(x, y - 1, start, end);
            PutInOpenSet(x + 1, y, start, end);
            PutInOpenSet(x - 1, y, start, end);

            PutInOpenSet(x - 1, y + 1, start, end);
            PutInOpenSet(x + 1, y + 1, start, end);
            PutInOpenSet(x - 1, y - 1, start, end);
            PutInOpenSet(x + 1, y - 1, start, end);
        }

        private void FindNeighborNodeFour(Node start, Node end)
        {
            // 四个方向
            int x = start.x;
            int y = start.y;
            PutInOpenSet(x, y + 1, start, end);
            PutInOpenSet(x, y - 1, start, end);
            PutInOpenSet(x + 1, y, start, end);
            PutInOpenSet(x - 1, y, start, end);
        }

        private void PutInOpenSet(int x, int y, Node fatherNode, Node targetNode)
        {
            if (x < 0 || x >= mapWidth) return;
            if (y < 0 || y >= mapHeight) return;

            Node currentNode = nodes[x, y];

            if (openSet.Contains(currentNode) || !currentNode.walkable || closeSet.Contains(currentNode)) return;

            currentNode.father = fatherNode;
            currentNode.g = currentNode.father.g + GetDistance(currentNode.father, currentNode);
            currentNode.h = GetDistance(currentNode, targetNode);
            currentNode.f = currentNode.g + currentNode.h;

            openSet.Add(currentNode);
        }

        #endregion
    }
}