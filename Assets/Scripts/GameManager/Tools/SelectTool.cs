using ChenChen_AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SelectTool : MonoBehaviour
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
        // 如果在建造模式中则不检测输入
        if (ChenChen_BuildingSystem.BuildingSystemManager.Instance.Tool.OnBuildMode) return;
        InputUpdate();
    }

    private void InputUpdate()
    {
        if (UnityEngine.Input.GetMouseButtonDown(0))
        {
            // 记录滑动开始的位置
            startPos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
        }

        if (UnityEngine.Input.GetMouseButton(0))
        {
            // 更新滑动结束的位置
            endPos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);

            // 如果滑动距离超过一定值就绘制方框
            if (Vector2.Distance(startPos, endPos) > 0.1f)
            {
                DrawLineBox(startPos, endPos);
            }
        }

        if (UnityEngine.Input.GetMouseButtonUp(0))
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

        // 执行选中逻辑, 任何一个逻辑成功，就返回，顺序为优先级
        // 判断有无选中棋子
        if (Logic_Pawn(hitColliders)) return;
        // 判断有无选中物体
        if (Logic_Thing(hitColliders)) return;

    }

    private bool Logic_Pawn(Collider2D[] hitColliders)
    {
        bool flag = false;
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Pawn"))
            {
                Pawn controller = collider.GetComponent<Pawn>();
                controller.IsSelect = !controller.IsSelect;
                flag = true;
            }
        }
        return flag;
    }
    private bool Logic_Thing(Collider2D[] hitColliders)
    {
        bool flag = false;
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Thing"))
            {
                DetailView_Thing detailView = collider.GetComponent<DetailView_Thing>();
                detailView.Selected();
                flag = true;
            }
        }
        return flag;
    }
}

