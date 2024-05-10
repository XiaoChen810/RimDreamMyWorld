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
            AddNewWorkSpace($"������{index++}", WorkSpaceType.Farm);
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
            // �������λ��
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // ����갴��ʱ��׼�������¹�������
            if (Input.GetMouseButtonDown(0))
            {
                mouseDownPosition = mousePosition;
                mouseDownPosition.x = Mathf.Ceil(mousePosition.x);
                mouseDownPosition.y = Mathf.Ceil(mousePosition.y);
            }
            // ������������ʱ����鵱ǰλ���Ƿ����Ҫ�󣬲���ʾ
            if (Input.GetMouseButton(0))
            {
                mousePosition.x = Mathf.Floor(mousePosition.x);
                mousePosition.y = Mathf.Floor(mousePosition.y);
                // ������갴�µ�λ�ú͵�ǰ��λ�û��Ʒ���
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
            // ������ɿ�ʱ��ȷ�Ϸ���
            if (Input.GetMouseButtonUp(0))
            {
                // ����sr�Ĵ�СΪ�����С
                mousePosition.x = Mathf.Floor(mousePosition.x);
                mousePosition.y = Mathf.Floor(mousePosition.y);
                workSpace.SetSize(mouseDownPosition, mousePosition);
                workSpace.gameObject.SetActive(true);
                // ����LineRenderer
                ResetLineRenderer();
                IsDoingWorkSpace = false;
            }
            // ������Ҽ����£�ȡ�����ι���������
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
                // ���κ�һ�������Լ�����ײ��ͷ���
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
            // �����ߵĶ���
            Vector3[] positions = new Vector3[5];
            positions[0] = new Vector3(start.x, start.y, 0);
            positions[1] = new Vector3(end.x, start.y, 0);
            positions[2] = new Vector3(end.x, end.y, 0);
            positions[3] = new Vector3(start.x, end.y, 0);
            positions[4] = new Vector3(start.x, start.y, 0);

            // �����ߵĶ���
            lineRenderer.positionCount = 5;
            lineRenderer.SetPositions(positions);
        }

        void ResetLineRenderer()
        {
            // �����߿�
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
    /// ���һ���µĹ�����
    /// </summary>
    private void AddNewWorkSpace(string newWorkSpaceName, WorkSpaceType workSpaceType)
    {
        if (TotalWorkSpaceDictionary.ContainsKey(newWorkSpaceName))
        {
            Debug.LogWarning("����������ͬ���ֵĹ�����");
            return;
        }
        // ����ʵ��
        GameObject obj = Instantiate(WorkSpacePrefab, transform);
        obj.name = newWorkSpaceName;
        obj.tag = "WorkSpace";
        WorkSpace workSpace = obj.GetComponent<WorkSpace>();
        workSpace.Init(workSpaceType);
        TotalWorkSpaceDictionary.Add(newWorkSpaceName, workSpace);
        // ��ʼ����
        StartCoroutine(WorkSpaceSettingCo(newWorkSpaceName));
    }

    /// <summary>
    /// �������й�����
    /// </summary>
    private void ExpendWorkSpace()
    {

    }

    /// <summary>
    /// ��С���й�����
    /// </summary>
    private void NarrowWorkSpace()
    {

    }

    /// <summary>
    /// ɾ�����й�����
    /// </summary>
    private void DeleteWorkSpace()
    {

    }
}
