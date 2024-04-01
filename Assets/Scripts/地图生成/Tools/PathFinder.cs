using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_MapGenerator
{
    public class PathFinder
    {
        private HashSet<FinderNode> openSet;
        private HashSet<FinderNode> closeSet;

        // ��ͼ���
        private int mapWidth;
        private int mapHeight;

        // ƫ����
        private float offset;

        // ȫ�ֵĽڵ��б�
        private FinderNode[,] nodes;

        public PathFinder(int mapWidth, int mapHeight)
        {
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            offset = 0.5f;
            openSet = new HashSet<FinderNode>();
            closeSet = new HashSet<FinderNode>();
        }

        #region Public

        /// <summary>
        ///  ��ʼ���ڵ��б�
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="numberUnitGrid"></param>
        /// <returns></returns>
        public SceneMapData InitNodes(int width, int height, SceneMapData mapData)
        {
            FinderNode[,] nodes = new FinderNode[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    bool set = true;

                    // �����ж�����
                    if(mapData.mapNodes[i, j ].type == MapNode.Type.water)
                        set = false;
                    if(mapData.obstaclesPositionList.Contains(new Vector3(i,j)))
                        set = false;

                    // ����
                    nodes[i, j] = new FinderNode(i, j, set ? 0 : FinderNode.s_MaxIntoCost);
                }
            }

            mapData.finderNodes = nodes;
            return mapData;
        }

        /// <summary>
        ///  ����һ��·���б�
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="targetPos"></param>
        /// <param name="mapName"></param>
        /// <returns></returns>
        public List<Vector2> FindPath(Vector3 startPos, Vector3 targetPos, FinderNode[,] mapNodes)
        {
            nodes = mapNodes;

            // ��λ��ת���ɽڵ���Ϣ
            FinderNode startNode = TransfromVectorToNode(startPos);
            FinderNode endNode = TransfromVectorToNode(targetPos);

            // �ж��Ƿ����
            if (endNode == null || !endNode.walkable)
            {
                Debug.LogWarning($"Ŀ���{targetPos}���ɴ�");
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
                FinderNode currentNode = null;

                foreach (var node in openSet)
                {
                    if (node == endNode)
                    {
                        currentNode = node;
                        break;
                    }
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

                        Vector2 wayPoint = new Vector2(endNode.cols , endNode.rows)
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
        /// ����ĳ���ڵ�Ľ������ģ�Ĭ��Ϊ��󣬼�����ΪΪ����ͨ�� 
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="pos"></param>
        /// <param name="set"></param>
        public void SetNodeIntoCost(FinderNode[,] nodes, Vector3Int pos, float set = FinderNode.s_MaxIntoCost)
        {
            nodes[pos.x, pos.y].intoCost = set;
        }


        #endregion

        #region AStar

        private FinderNode TransfromVectorToNode(Vector3 pos)
        {
            // �жϷ�Χ 
            if (pos.x < 0 || pos.x >= mapWidth) return null;
            if (pos.y < 0 || pos.y >= mapHeight) return null;

            int x = Mathf.FloorToInt(pos.x);
            int y = Mathf.FloorToInt(pos.y);

            if (x < 0 || x >= mapWidth) return null;
            if (y < 0 || y >= mapHeight) return null;

            return nodes[x, y];
        }

        private float GetDistance(FinderNode a, FinderNode b)
        {
            float result;
            result = Mathf.Abs(a.cols - b.cols) + Mathf.Abs(a.rows - b.rows);
            return result;
        }

        private void FindNeighborNodeEight(FinderNode start, FinderNode end)
        {
            // �˸�����
            int x = start.cols;
            int y = start.rows;
            PutInOpenSet(x, y + 1, start, end);
            PutInOpenSet(x, y - 1, start, end);
            PutInOpenSet(x + 1, y, start, end);
            PutInOpenSet(x - 1, y, start, end);
            //PutInOpenSet(x - 1, y + 1, start, end);
            //PutInOpenSet(x + 1, y + 1, start, end);
            //PutInOpenSet(x - 1, y - 1, start, end);
            //PutInOpenSet(x + 1, y - 1, start, end);

            if (NodeWalkable(x - 1, y) && NodeWalkable(x, y + 1))
                PutInOpenSet(x - 1, y + 1, start, end);
            if (NodeWalkable(x + 1, y) && NodeWalkable(x, y + 1))
                PutInOpenSet(x + 1, y + 1, start, end);
            if (NodeWalkable(x - 1, y) && NodeWalkable(x, y - 1))
                PutInOpenSet(x - 1, y - 1, start, end);
            if (NodeWalkable(x + 1, y) && NodeWalkable(x, y - 1))
                PutInOpenSet(x + 1, y - 1, start, end);
        }

        private bool NodeWalkable(int x,int y)
        {
            if (x < 0 || x >= mapWidth) return false;
            if (y < 0 || y >= mapHeight) return false;
            return nodes[x,y].walkable;
        }

        private void PutInOpenSet(int x, int y, FinderNode fatherNode, FinderNode targetNode)
        {
            if (x < 0 || x >= mapWidth) return;
            if (y < 0 || y >= mapHeight) return;

            FinderNode currentNode = nodes[x, y];

            if (openSet.Contains(currentNode) || !currentNode.walkable || closeSet.Contains(currentNode)) return;

            currentNode.father = fatherNode;
            currentNode.Gcost = currentNode.father.Gcost + GetDistance(currentNode.father, currentNode) + currentNode.intoCost;
            currentNode.Hcost = GetDistance(currentNode, targetNode);
            currentNode.Fcost = currentNode.Gcost + currentNode.Hcost;

            openSet.Add(currentNode);
        }

        private void FindNeighborNodeFour(FinderNode start, FinderNode end)
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