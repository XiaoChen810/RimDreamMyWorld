using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGridLine : MonoBehaviour
{
    public int gridSize = 1; // �����С
    public int gridWidth = 20; // ������
    public int gridHeight = 20; // ����߶�

    void OnDrawGizmos()
    {
        DrawGrid();
    }

    void DrawGrid()
    {
        Vector3 origin = new Vector3(0, 0, 0); // ������ʼ��

        // ���ƴ�ֱ��
        for (float x = origin.x; x < gridWidth; x += gridSize)
        {
            Gizmos.DrawLine(new Vector3(x, origin.y, origin.z), new Vector3(x, gridHeight, origin.z));
        }

        // ����ˮƽ��
        for (float y = origin.y; y < gridHeight; y += gridSize)
        {
            Gizmos.DrawLine(new Vector3(origin.x, y, origin.z), new Vector3(gridWidth, y, origin.z));
        }
    }
}

