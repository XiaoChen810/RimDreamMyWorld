using System;
using System.Collections.Generic;
using TreeEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Quadtree
{
    private const int MAX_OBJECTS = 5; // 每个节点中最多包含的对象数量
    private const int MAX_LEVELS = 6; // 四叉树的最大深度

    private int level; // 当前节点的深度
    private GameObject parent;// 当前节点的父对象   
    private GameObject self;//当前节点的对象 
    private List<GameObject> objects; // 当前节点中包含的对象列表
    private Rect bounds; // 当前节点的边界
    private Quadtree[] nodes; // 四个子节点

    // 构造函数，初始化节点
    public Quadtree(int level, Rect bounds, GameObject parent, string Name)
    {
        this.level = level;
        this.bounds = bounds;
        this.objects = new List<GameObject>();
        this.nodes = new Quadtree[4];
        this.self = new GameObject(Name);
        this.self.transform.parent = parent.transform;
        this.parent = parent;
    }

    // 清空当前节点及其所有子节点
    public void Clear()
    {
        objects.Clear();

        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i] != null)
            {
                nodes[i].Clear();
                nodes[i] = null;
            }
        }
    }

    // 分割当前节点，创建四个子节点
    private void Split()
    {
        float subWidth = bounds.width / 2f;
        float subHeight = bounds.height / 2f;
        float x = bounds.x;
        float y = bounds.y;

        nodes[0] = new Quadtree(level + 1, new Rect(x + subWidth, y, subWidth, subHeight), self, $"右下 level{level}");
        nodes[1] = new Quadtree(level + 1, new Rect(x, y, subWidth, subHeight), self, $"左下 level{level}");
        nodes[2] = new Quadtree(level + 1, new Rect(x, y + subHeight, subWidth, subHeight), self, $"左上 level{level}");
        nodes[3] = new Quadtree(level + 1, new Rect(x + subWidth, y + subHeight, subWidth, subHeight), self, $"右上 level{level}");
    }

    // 获取对象应插入的子节点索引
    private int GetIndex(GameObject go)
    {
        int index = -1;
        Vector2 pos = go.transform.position;
        float verticalMidpoint = bounds.x + bounds.width / 2f;
        float horizontalMidpoint = bounds.y + bounds.height / 2f;

        bool topQuadrant = pos.y >= horizontalMidpoint; // 对象在上半部分
        bool bottomQuadrant = pos.y <= horizontalMidpoint; // 对象在下半部分
        bool leftQuadrant = pos.x <= verticalMidpoint; // 对象在左半部分
        bool rightQuadrant = pos.x >= verticalMidpoint; // 对象在右半部分

        if (topQuadrant)
        {
            if (leftQuadrant)
            {
                index = 2;
            }
            else if (rightQuadrant)
            {
                index = 3;
            }
        }
        else if (bottomQuadrant)
        {
            if (leftQuadrant)
            {
                index = 1;
            }
            else if (rightQuadrant)
            {
                index = 0;
            }
        }

        return index;
    }

    // 插入对象到四叉树中
    public void Insert(GameObject go)
    {
        // 如果当前节点有子节点，则尝试将对象插入到子节点中
        if (nodes[0] != null)
        {
            int index = GetIndex(go);

            if (index != -1)
            {
                nodes[index].Insert(go);
                return;
            }
        }

        // 如果当前节点没有子节点或对象无法放入子节点，则放入当前节点
        objects.Add(go);
        go.transform.parent = this.self.transform;

        // 如果当前节点中的对象数量超过最大限制且未达到最大深度，则分割节点
        if (objects.Count > MAX_OBJECTS && level < MAX_LEVELS)
        {
            if (nodes[0] == null)
            {
                Split();
            }

            int i = 0;
            while (i < objects.Count)
            {
                int index = GetIndex(objects[i]);
                if (index != -1)
                {
                    go.transform.parent = null;
                    nodes[index].Insert(objects[i]);
                    objects.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }

    // 剔除除了指定范围内的所有对象
    public void Retrieve(Rect range)
    {
        if (!bounds.Overlaps(range))
        {
            // 当前节点的边界与范围不重叠，直接返回
            self.SetActive(false);
            return;
        }

        // 当前节点的边界与范围重叠
        self.SetActive(true);

        if (nodes[0] != null)
        {
            // 在子节点中检索，并缩小范围
            foreach (var node in nodes)
            {
                node.Retrieve(range);
            }
        }
        else
        {
            //self.SetActive(true);
            return;
        }
    }

    // 获取范围包含的子节点索引（用于范围检索）
    private List<int> GetIndex(Rect range)
    {
        List<int> indexes = new List<int>();
        float verticalMidpoint = bounds.x + bounds.width / 2f;
        float horizontalMidpoint = bounds.y + bounds.height / 2f;

        bool top = range.yMax > horizontalMidpoint;
        bool bottom = range.yMin < horizontalMidpoint;
        bool left = range.xMin < verticalMidpoint;
        bool right = range.xMax > verticalMidpoint;

        if (top)
        {
            if (left) indexes.Add(2); // 左上
            if (right) indexes.Add(3); // 右上
        }
        if (bottom)
        {
            if (left) indexes.Add(1); // 左下
            if (right) indexes.Add(0); // 右下
        }

        return indexes;
    }
}
