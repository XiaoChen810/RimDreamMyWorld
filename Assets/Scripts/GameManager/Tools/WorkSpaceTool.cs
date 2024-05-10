using AYellowpaper.SerializedCollections;
using ChenChen_MapGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkSpaceTool : MonoBehaviour
{
    public GameManager GameManager;
    public GameObject WorkSpacePrefab;
    private LineRenderer lineRenderer;

    [SerializedDictionary("SpaceName", "WorkSpace")]
    public SerializedDictionary<string, WorkSpace> TotalWorkSpaceDictionary;

    public bool IsDoingWorkSpace = false;

    private void Start()
    {
        GameManager = GetComponent<GameManager>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    private int index = 0;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            AddNewWorkSpace($"工作区{index++}", WorkSpaceType.Farm);
        }
    }

    public GameObject GetAWorkSpace(WorkSpaceType workSpaceType)
    {
        GameObject result = null;
        foreach(var space in TotalWorkSpaceDictionary)
        {
            if(space.Value.Type == workSpaceType && !space.Value.IsUsed)
            {
                result = space.Value.gameObject;
                break;
            }
        }
        return result;
    }

    private IEnumerator WorkSpaceSettingCo(string workSpaceName)
    {
        WorkSpace workSpace;
        if (!TotalWorkSpaceDictionary.TryGetValue(workSpaceName, out workSpace))
        {
            IsDoingWorkSpace = false;
            yield break;
        }

        IsDoingWorkSpace = true;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseDownPosition = mousePosition;
        workSpace.transform.position = mouseDownPosition;
        workSpace.gameObject.SetActive(false);

        while (IsDoingWorkSpace)
        {
            // 监听鼠标位置
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 当鼠标按下时，准备放置新工作区块
            if (Input.GetMouseButtonDown(0))
            {
                mouseDownPosition = mousePosition;
                mouseDownPosition.x = Mathf.Ceil(mousePosition.x);
                mouseDownPosition.y = Mathf.Ceil(mousePosition.y);
            }
            // 当鼠标持续按下时，检查当前位置是否符合要求，并提示
            if (Input.GetMouseButton(0))
            {
                mousePosition.x = Mathf.Floor(mousePosition.x);
                mousePosition.y = Mathf.Floor(mousePosition.y);
                // 根据鼠标按下的位置和当前的位置绘制方框
                if (IsOk(mouseDownPosition, mousePosition))
                {
                    ChangeColor(Color.green);
                }
                else
                {
                    ChangeColor(Color.red);
                }
                DrawLineBox(mouseDownPosition, mousePosition);
            }
            // 当鼠标松开时，确认放置
            if (Input.GetMouseButtonUp(0))
            {
                // 设置sr的大小为方框大小
                mousePosition.x = Mathf.Floor(mousePosition.x);
                mousePosition.y = Mathf.Floor(mousePosition.y);
                workSpace.SetSize(mouseDownPosition, mousePosition);
                workSpace.gameObject.SetActive(true);
                // 重置LineRenderer
                ResetLineRenderer();
                IsDoingWorkSpace = false;
            }
            // 当鼠标右键按下，取消本次工作区设置
            if (Input.GetMouseButtonDown(1))
            {
                ResetLineRenderer();
                Destroy(workSpace.gameObject);
                TotalWorkSpaceDictionary.Remove(workSpaceName);
                IsDoingWorkSpace = false;
                break;
            }
            yield return null;
        }

        bool IsOk(Vector2 start, Vector2 end)
        {
            if (Mathf.Abs(start.x - end.x) < 1) return false;
            if (Mathf.Abs(start.y - end.y) < 1) return false;
            Collider2D[] hitColliders = Physics2D.OverlapAreaAll(start, end);
            foreach (Collider2D collider in hitColliders)
            {
                // 有任何一个不是自己的碰撞体就返回
                if (collider != workSpace.Coll) return false;
            }
            var nodes = MapManager.Instance.MapDatasDict[MapManager.Instance.CurrentMapName].mapNodes;
            int minx = start.x < end.x ? (int)start.x : (int)end.x;
            int maxx = start.x > end.x ? (int)start.x : (int)end.x;
            int miny = start.y < end.y ? (int)start.y : (int)end.y;
            int maxy = start.y > end.y ? (int)start.y : (int)end.y;
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

        void DrawLineBox(Vector2 start, Vector2 end)
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
    }

    /// <summary>
    /// 添加一个新的工作区
    /// </summary>
    private void AddNewWorkSpace(string newWorkSpaceName, WorkSpaceType workSpaceType)
    {
        if (TotalWorkSpaceDictionary.ContainsKey(newWorkSpaceName))
        {
            Debug.LogWarning("不会生成相同名字的工作区");
            return;
        }
        // 生成实例
        GameObject obj = Instantiate(WorkSpacePrefab, transform);
        obj.name = newWorkSpaceName;
        obj.tag = "WorkSpace";
        WorkSpace workSpace = obj.GetComponent<WorkSpace>();
        workSpace.Init(workSpaceType);
        TotalWorkSpaceDictionary.Add(newWorkSpaceName, workSpace);
        // 初始设置
        StartCoroutine(WorkSpaceSettingCo(newWorkSpaceName));
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
}
