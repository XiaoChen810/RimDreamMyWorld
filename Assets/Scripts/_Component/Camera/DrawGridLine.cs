using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGridLine : MonoBehaviour
{
    public int gridSize = 1; // 网格大小
    public int gridWidth = 20; // 网格宽度
    public int gridHeight = 20; // 网格高度

    void OnDrawGizmos()
    {
        DrawGrid();
    }

    void DrawGrid()
    {
        Vector3 origin = new Vector3(0, 0, 0); // 网格起始点

        // 绘制垂直线
        for (float x = origin.x; x < gridWidth; x += gridSize)
        {
            Gizmos.DrawLine(new Vector3(x, origin.y, origin.z), new Vector3(x, gridHeight, origin.z));
        }

        // 绘制水平线
        for (float y = origin.y; y < gridHeight; y += gridSize)
        {
            Gizmos.DrawLine(new Vector3(origin.x, y, origin.z), new Vector3(gridWidth, y, origin.z));
        }
    }
}

