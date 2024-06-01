using AYellowpaper.SerializedCollections;
using ChenChen_Map;
using ChenChen_Thing;
using ChenChen_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkSpaceTool : MonoBehaviour
{
    private LineRenderer lineRenderer;

    [SerializedDictionary("SpaceName", "WorkSpace")]
    public SerializedDictionary<string, WorkSpace> TotalWorkSpaceDictionary;

    public GameObject WorkSpacePrefab;
    public bool IsDoingWorkSpace = false;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    private int index = 0;
    
    /// <summary>
    /// 获取一种类型的工作区的GameObject
    /// </summary>
    /// <param name="workSpaceType"></param>
    /// <returns></returns>
    public GameObject GetAWorkSpace(WorkSpaceType workSpaceType)
    {
        GameObject result = null;
        foreach(var space in TotalWorkSpaceDictionary)
        {
            if(space.Value.WorkSpaceType == workSpaceType 
                && space.Value.Permission == PermissionBase.PermissionType.IsFree)
            {
                result = space.Value.gameObject;
                break;
            }
        }
        return result;
    }

    /// <summary>
    /// 添加一个新的 种植 工作区
    /// </summary>
    public void AddNewWorkSpace(string newWorkSpaceName, string cropName)
    {
        newWorkSpaceName = newWorkSpaceName + index++.ToString();
        if (TotalWorkSpaceDictionary.ContainsKey(newWorkSpaceName))
        {
            Debug.LogWarning("不会生成相同名字的工作区");
            return;
        }

        Action<GameObject> onPlace = (GameObject obj) =>
        {
            if (obj.TryGetComponent<WorkSpace_Farm>(out WorkSpace_Farm workSpace_Farm))
            {
                workSpace_Farm.Init(cropName);
            }
        };

        // 初始设置
        StartCoroutine(AddWorkSpaceCo(newWorkSpaceName, onPlace));
    }

    public void LoadFarmWorkSpaceFromSave(Data_GameSave gameSave)
    {
        foreach(var fws in gameSave.SaveFarmWorkSpace)
        {
            // 加载种植区。
            Vector2 vector_bl = new Vector2(fws.bounds.min.x, fws.bounds.min.y);
            Vector2 vector_tr = new Vector2(fws.bounds.max.x, fws.bounds.max.y);
            GameObject go = WorkSpacePlacedFinally(fws.workSpaceName, vector_bl, vector_tr);
            WorkSpace_Farm wsf = go.GetComponent<WorkSpace_Farm>();
            wsf.Init(fws.cropName);
            // 将其已经种植的作物加载
            foreach(var cropSave in fws.crops)
            {
                wsf.SetAPositionHadFarmed(cropSave);
            }
        }

    }

    /// <summary>
    /// 扩张现有工作区
    /// </summary>
    private void ExpendWorkSpace()
    {

    }

    /// <summary>
    /// 缩小现有工作区
    /// </summary>
    private void NarrowWorkSpace()
    {

    }

    /// <summary>
    /// 删除现有工作区
    /// </summary>
    private void DeleteWorkSpace()
    {

    }

    private IEnumerator AddWorkSpaceCo(string newWorkSpaceName, Action<GameObject> onComplete)
    {
        IsDoingWorkSpace = true;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseDownPosition = mousePosition;

        bool flag = false;
        while (IsDoingWorkSpace)
        {
            // 监听鼠标位置
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 当鼠标按下时，准备放置新工作区块
            if (Input.GetMouseButtonDown(0))
            {
                mouseDownPosition = mousePosition;
                flag = true;
            }

            Vector2 pointA = mousePosition;
            Vector2 pointB = flag ? mouseDownPosition : mousePosition;

            // 计算矩形的边界
            float minX = Mathf.Floor(Mathf.Min(pointA.x, pointB.x));
            float maxX = Mathf.Ceil(Mathf.Max(pointA.x, pointB.x));
            float minY = Mathf.Floor(Mathf.Min(pointA.y, pointB.y));
            float maxY = Mathf.Ceil(Mathf.Max(pointA.y, pointB.y));

            // 确保矩形的宽度和高度至少为1
            if (maxX - minX < 1)
            {
                maxX = minX + 1;
            }
            if (maxY - minY < 1)
            {
                maxY = minY + 1;
            }

            // 计算矩形的四个顶点
            Vector2 topLeft = new Vector2(minX, maxY);
            Vector2 topRight = new Vector2(maxX, maxY);
            Vector2 bottomLeft = new Vector2(minX, minY);
            Vector2 bottomRight = new Vector2(maxX, minY);

            // 设置顶点位置
            Vector3[] corners = new Vector3[]
            {
            new Vector3(topLeft.x, topLeft.y, 0),
            new Vector3(topRight.x, topRight.y, 0),
            new Vector3(bottomRight.x, bottomRight.y, 0),
            new Vector3(bottomLeft.x, bottomLeft.y, 0),
            new Vector3(topLeft.x, topLeft.y, 0) // 回到起点
            };

            // 当鼠标未按下时
            if (!flag)
            {
                if (IsOk(bottomLeft, topRight))
                {
                    ChangeColor(Color.green);
                }
                else
                {
                    ChangeColor(Color.red);
                }
                DrawLineBox(corners);
            }

            // 当鼠标持续按下时，检查当前位置是否符合要求，并提示
            if (Input.GetMouseButton(0) && flag)
            {
                // 根据鼠标按下的位置和当前的位置绘制方框
                if (IsOk(bottomLeft, topRight))
                {
                    ChangeColor(Color.green);
                }
                else
                {
                    ChangeColor(Color.red);
                }
                DrawLineBox(corners);
            }
            // 当鼠标松开时，确认放置
            if (Input.GetMouseButtonUp(0) && flag)
            {
                if (IsOk(mouseDownPosition, mouseDownPosition))
                {
                    GameObject obj = WorkSpacePlacedFinally(newWorkSpaceName, bottomLeft, topRight);
                    // 重置LineRenderer
                    ResetLineRenderer();
                    IsDoingWorkSpace = false;

                    onComplete?.Invoke(obj);
                }
                else
                {
                    Cancel();
                }
            }
            // 当鼠标右键按下，取消本次工作区设置
            if (Input.GetMouseButtonDown(1))
            {
                Cancel();
                break;
            }
            yield return null;
        }

        bool IsOk(Vector2 bl, Vector2 tr)
        {
            // 计算矩形范围, 剔除边线
            Vector2 pa = bl;
            pa.x += 0.05f;
            pa.y += 0.05f;
            Vector2 pb = tr;
            pb.x += -0.05f;
            pb.y += -0.05f;
            Collider2D[] hitColliders = Physics2D.OverlapAreaAll(pa, pb);
            foreach (Collider2D collider in hitColliders)
            {
                // 有任何一个碰撞体就返回
                return false;
            }
            var nodes = MapManager.Instance.CurMapNodes;
            int minx = bl.x < tr.x ? (int)bl.x : (int)tr.x;
            int maxx = bl.x > tr.x ? (int)bl.x : (int)tr.x;
            int miny = bl.y < tr.y ? (int)bl.y : (int)tr.y;
            int maxy = bl.y > tr.y ? (int)bl.y : (int)tr.y;
            for (int i = minx; i < maxx + 1; i++)
            {
                for (int j = miny; j < maxy + 1; j++)
                {
                    if (nodes[i, j].type == NodeType.water)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        void DrawLineBox(Vector3[] corners)
        {
            // 设置线的顶点
            lineRenderer.positionCount = 5;
            lineRenderer.SetPositions(corners);
        }

        void ResetLineRenderer()
        {
            // 重置线框
            lineRenderer.positionCount = 0;
            ChangeColor(Color.white);
        }

        void ChangeColor(Color color)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }

        void Cancel()
        {
            ResetLineRenderer();
            index--;
            IsDoingWorkSpace = false;
        }
    }

    private GameObject WorkSpacePlacedFinally(string newWorkSpaceName, Vector2 vector_bl, Vector2 vector_tr)
    {
        // 生成实例
        GameObject obj = Instantiate(WorkSpacePrefab, transform);
        obj.name = newWorkSpaceName;
        obj.tag = "WorkSpace";
        WorkSpace workSpace = obj.GetComponent<WorkSpace>();

        // 设置sr的大小为方框大小
        workSpace.SetSize(vector_tr, vector_bl);
        workSpace.gameObject.SetActive(true);

        TotalWorkSpaceDictionary.Add(newWorkSpaceName, workSpace);
        return obj;
    }
}
