using ChenChen_AI;
using ChenChen_Thing;
using ChenChen_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class SelectTool : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Vector2 startPos;
    private Vector2 endPos;
    private bool isDragging = false;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        InitializeLineRenderer();
    }

    private void Update()
    {
        // 如果在建造模式中则不检测输入
        if (ThingSystemManager.Instance.Tool.OnBuildMode) return;
        // 如果正在放置工作区则不检测输入
        if (GameManager.Instance.WorkSpaceTool.IsDoingWorkSpace) return;
        InputUpdate();
    }

    private void InputUpdate()
    {
        if (UnityEngine.Input.GetMouseButtonDown(0))
        {
            // 检查鼠标是否在UI上
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            // 记录滑动开始的位置
            startPos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            isDragging = true;
        }

        if (UnityEngine.Input.GetMouseButton(0))
        {
            if (!isDragging) return;

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
            if (!isDragging) return;

            // 在滑动结束时处理多选
            HandleSelection(startPos, endPos);
            ResetLineRenderer();
            isDragging = false;
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

    private List<DetailView> dvs = new();
    private void HandleSelection(Vector2 start, Vector2 end)
    {
        // 在这里处理多选逻辑
        Collider2D[] hitColliders = Physics2D.OverlapAreaAll(start, end);

        // 执行选中逻辑, 任何一个逻辑成功，就返回，顺序为优先级
        foreach(var d in dvs)
        {
            //关闭上一次DetailView的选择
            if (d != null)
            {
                d.CloseIndicator();
                d.ClosePanel();
            }
        }
        dvs.Clear();

        if (Logic_Pawn(hitColliders)) return;
        if (Logic_Animal(hitColliders)) return;
        if (Logic_Thing(hitColliders)) return;
        if (Logic_Wall(hitColliders)) return;
        if (Logic_Floor(hitColliders)) return;
        if (Logic_WorkSpace(hitColliders)) return;
    }

    private bool Logic_Pawn(Collider2D[] hitColliders)
    {
        bool flag = false;
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Pawn"))
            {
                Pawn pawn = collider.GetComponent<Pawn>();
                dvs.Add(pawn.DetailView);
                flag = true;
            }
        }
        DetailViewOpen();
        return flag;
    }
    private bool Logic_Animal(Collider2D[] hitColliders)
    {
        bool flag = false;
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Animal"))
            {
                Animal animal = collider.GetComponent<Animal>();
                dvs.Add(animal.DetailView);
                flag = true;
            }
        }
        DetailViewOpen();
        return flag;
    }
    private bool Logic_Thing(Collider2D[] hitColliders)
    {
        bool flag = false;
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Thing"))
            {
                Thing thing = collider.GetComponent<Thing>();
                dvs.Add(thing.DetailView);
                flag = true;
            }
        }
        DetailViewOpen();
        return flag;
    }
    private bool Logic_Wall(Collider2D[] hitColliders)
    {
        bool flag = false;
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Wall"))
            {
                Thing thing = collider.GetComponent<Thing_Wall>();
                dvs.Add(thing.DetailView);
                flag = true;
            }
        }
        DetailViewOpen();
        return flag;
    }
    private bool Logic_Floor(Collider2D[] hitColliders)
    {
        bool flag = false;
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Floor"))
            {
                Thing thing = collider.GetComponent<Thing_Floor>();
                dvs.Add(thing.DetailView);
                flag = true;
            }
        }
        DetailViewOpen();
        return flag;
    }
    private bool Logic_WorkSpace(Collider2D[] hitColliders)
    {
        bool flag = false;
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("WorkSpace"))
            {
                WorkSpace workSpace = collider.GetComponent<WorkSpace>();
                dvs.Add(workSpace.DetailView);
                flag = true;
            }
        }
        DetailViewOpen();
        return flag;
    }

    private void DetailViewOpen()
    {
        if (dvs.Count == 1)
        {
            dvs[0].OpenIndicator();
            dvs[0].OpenPanel();
        }
        else if (dvs.Count > 1)
        {
            foreach (var d in dvs)
            {
                d.OpenIndicator();
            }
        }
    }
}

