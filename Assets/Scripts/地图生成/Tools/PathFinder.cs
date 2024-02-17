using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MyMapGenerate
{
    public class PathFinder
    {
        /// <summary>
        ///  Ѱ·�㷨�Ľڵ㣬Ҫ����<see langword="bool"/> walkable �ж��Ƿ����ͨ��
        /// </summary>
        public class Node
        {
            public int cols, rows;
            public float Fcost, Gcost, Hcost;
            public Node father;
            public bool walkable;

            public Node(int x, int y, bool walkable)
            {
                this.cols = x;
                this.rows = y;
                this.walkable = walkable;
            }
        }

        // �����б�͹ر��б�
        //private List<Node> openSet;
        //private List<Node> closeSet;

        private HashSet<Node> openSet;
        private HashSet<Node> closeSet;
        private HashSet<Node> visited;


        // ��ͼ���
        int mapWidth;
        int mapHeight;

        // �����ܳ�
        int gridWidth;
        int gridHeight;

        // ��λ��������
        int numberUnitGrid;

        // ƫ����
        float offset;

        // ȫ�ֵĽڵ��б�
        Node[,] nodes;

        public PathFinder(int mapWidth, int mapHeight, int numberUnitGrid)
        {
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            this.numberUnitGrid = numberUnitGrid;
            this.gridWidth = mapWidth * numberUnitGrid;
            this.gridHeight = mapHeight * numberUnitGrid;
            offset = 1 / numberUnitGrid * 0.5f;
            openSet = new HashSet<Node>();
            closeSet = new HashSet<Node>();
        }

        #region Public

        /// <summary>
        ///  ��ʼ��һ���ڵ��б�
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="numberUnitGrid"></param>
        /// <returns></returns>
        public Node[,] InitNodes(int width, int height, int numberUnitGrid, MapCreator.TileData[,] mapTileDatas)
        {
            Node[,] nodes = new Node[width * numberUnitGrid, height * numberUnitGrid];

            for (int i = 0; i < width * numberUnitGrid; i++)
            {
                for (int j = 0; j < height * numberUnitGrid; j++)
                {
                    bool set = true;
                    if(mapTileDatas[i / numberUnitGrid, j / numberUnitGrid].type == MapCreator.TileData.Type.water)
                        set = false;
                    if(mapTileDatas[i / numberUnitGrid, j / numberUnitGrid].walkAble == false)
                        set = false;
                    nodes[i, j] = new Node(i, j, set);
                }
            }

            return nodes;
        }

        /// <summary>
        ///  ����һ��·���б�
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="targetPos"></param>
        /// <param name="mapName"></param>
        /// <returns></returns>
        public List<Vector2> FindPath(Vector3 startPos, Vector3 targetPos, Node[,] mapNodes)
        {
            nodes = mapNodes;

            // ��λ��ת���ɽڵ���Ϣ
            Node startNode = TransfromVectorToNode(startPos);
            Node endNode = TransfromVectorToNode(targetPos);

            // �ж��Ƿ����
            if (endNode == null || endNode.walkable != true)
            {
                // UnityEngine.Debug.Log($"Ŀ���{targetPos}���ɴ�");
                return null;
            }

            // ��ʼ��
            startNode.Fcost = 0;
            startNode.Gcost = 0;
            startNode.Hcost = 0;
            startNode.father = null;

            openSet.Clear();
            closeSet.Clear();

            openSet.Add(startNode);
            while (openSet.Count > 0)
            {
                // Ѱ����С���۵Ľڵ�
                Node currentNode = null;

                foreach (var node in openSet)
                {
                    if (currentNode == null || node.Fcost < currentNode.Fcost || (node.Fcost == currentNode.Fcost && node.Hcost < currentNode.Hcost))
                    {
                        currentNode = node;
                    }
                }

                // ���ڵ��ƶ���close�б� 
                openSet.Remove(currentNode);
                closeSet.Add(currentNode);

                if (currentNode == endNode)
                {
                    // ���������Close�б�����·���б�
                    List<Vector2> rawPath = new List<Vector2>();

                    while (endNode.father != null)
                    {

                        Vector2 wayPoint = new Vector2(endNode.cols / numberUnitGrid, endNode.rows / numberUnitGrid)
                            + new Vector2(offset, offset);
                        rawPath.Add(wayPoint);
                        endNode = endNode.father;
                    }

                    rawPath.Reverse();

                    // ƽ��·��
                    List<Vector2> smoothPath = SmoothPath(rawPath);

                    return smoothPath;
                }


                // ��������ھ�����
                FindNeighborNodeEight(currentNode, endNode);
            }

            return null;

            List<Vector2> SmoothPath(List<Vector2> path)
            {
                List<Vector2> smoothPath = new List<Vector2>();

                if (path.Count < 2)
                {
                    // ���·��̫�̣�����Ҫƽ��
                    return path;
                }

                smoothPath.Add(path[0]);

                for (int i = 1; i < path.Count - 1; i++)
                {
                    Vector2 currentPoint = path[i];
                    Vector2 nextPoint = path[i + 1];

                    // ʹ�ñ��������߲�ֵ
                    Vector2 interpolatedPoint1 = Vector2.LerpUnclamped(currentPoint, nextPoint, 0.25f);
                    Vector2 interpolatedPoint2 = Vector2.LerpUnclamped(currentPoint, nextPoint, 0.5f);
                    Vector2 interpolatedPoint3 = Vector2.LerpUnclamped(currentPoint, nextPoint, 0.75f);

                    smoothPath.Add(interpolatedPoint1);
                    smoothPath.Add(interpolatedPoint2);
                    smoothPath.Add(interpolatedPoint3);
                }

                smoothPath.Add(path[path.Count - 1]);

                return smoothPath;
            }
        }

        /// <summary>
        /// ����Ѱ·�ڵ��Ƿ����ͨ�� 
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="pos"></param>
        /// <param name="set"></param>
        public void SetNodeWalkable(Node[,] nodes, Vector3Int pos, bool set = false)
        {
            int x = pos.x * numberUnitGrid;
            int y = pos.y * numberUnitGrid;
            nodes[x, y].walkable = set;
        }


        #endregion

        #region AStar

        private Node TransfromVectorToNode(Vector3 pos)
        {
            // �жϷ�Χ 
            if (pos.x < 0 || pos.x >= mapWidth) return null;
            if (pos.y < 0 || pos.y >= mapHeight) return null;

            int x = Mathf.FloorToInt(pos.x * numberUnitGrid);
            int y = Mathf.FloorToInt(pos.y * numberUnitGrid);

            if (x < 0 || x >= gridWidth) return null;
            if (y < 0 || y >= gridHeight) return null;

            return nodes[x, y];
        }

        private float GetDistance(Node a, Node b)
        {
            float result;
            result = Mathf.Abs(a.cols - b.cols) + Mathf.Abs(a.rows - b.rows);
            return result;
        }

        private void FindNeighborNodeEight(Node start, Node end)
        {
            // �˸�����
            int x = start.cols;
            int y = start.rows;
            PutInOpenSet(x, y + 1, start, end);
            PutInOpenSet(x, y - 1, start, end);
            PutInOpenSet(x + 1, y, start, end);
            PutInOpenSet(x - 1, y, start, end);

            if (nodes[x - 1, y].walkable && nodes[x, y + 1].walkable)
                PutInOpenSet(x - 1, y + 1, start, end);
            if (nodes[x + 1, y].walkable && nodes[x, y + 1].walkable)
                PutInOpenSet(x + 1, y + 1, start, end);
            if (nodes[x - 1, y].walkable && nodes[x, y - 1].walkable)
                PutInOpenSet(x - 1, y - 1, start, end);
            if (nodes[x + 1, y].walkable && nodes[x, y - 1].walkable)
                PutInOpenSet(x + 1, y - 1, start, end);
        }

        private void PutInOpenSet(int x, int y, Node fatherNode, Node targetNode)
        {
            if (x < 0 || x >= gridWidth) return;
            if (y < 0 || y >= gridHeight) return;

            Node currentNode = nodes[x, y];

            if (openSet.Contains(currentNode) || !currentNode.walkable || closeSet.Contains(currentNode)) return;

            currentNode.father = fatherNode;
            currentNode.Gcost = currentNode.father.Gcost + GetDistance(currentNode.father, currentNode);
            currentNode.Hcost = GetDistance(currentNode, targetNode);
            currentNode.Fcost = currentNode.Gcost + currentNode.Hcost;

            openSet.Add(currentNode);
        }

        private void FindNeighborNodeFour(Node start, Node end)
        {
            // �ĸ�����
            int x = start.cols;
            int y = start.rows;
            PutInOpenSet(x, y + 1, start, end);
            PutInOpenSet(x, y - 1, start, end);
            PutInOpenSet(x + 1, y, start, end);
            PutInOpenSet(x - 1, y, start, end);
        }

        #endregion
    }
}