using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SelectManager : MonoBehaviour
{
    private Vector2 startPos;
    private Vector2 endPos;
    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        InitializeLineRenderer();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 记录滑动开始的位置
            startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            // 更新滑动结束的位置
            endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 如果滑动距离超过一定值就绘制方框
            if (Vector2.Distance(startPos, endPos) > 0.1f)
            {
                DrawLineBox(startPos, endPos);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            // 在滑动结束时处理多选
            HandleSelection(startPos, endPos);
            ResetLineRenderer();
        }
    }

    private void InitializeLineRenderer()
    {
        // 设置线的宽度
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    private void DrawLineBox(Vector2 start, Vector2 end)
    {
        // 定义线的顶点
        Vector3[] positions = new Vector3[5];
        positions[0] = new Vector3(start.x, start.y, 0);
        positions[1] = new Vector3(end.x, start.y, 0);
        positions[2] = new Vector3(end.x, end.y, 0);
        positions[3] = new Vector3(start.x, end.y, 0);
        positions[4] = new Vector3(start.x, start.y, 0);

        // 设置线的顶点
        lineRenderer.positionCount = 5;
        lineRenderer.SetPositions(positions);
    }

    private void ResetLineRenderer()
    {
        // 重置线框
        lineRenderer.positionCount = 0;
    }

    private void HandleSelection(Vector2 start, Vector2 end)    
    {
        // 在这里处理多选逻辑
        Collider2D[] hitColliders = Physics2D.OverlapAreaAll(start, end);

        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Pawn"))
            {
                // 执行选中逻辑
                Pawn controller = collider.GetComponent<Pawn>();

                controller.TrySelect();           
            }
        }
    }
}

