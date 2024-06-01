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
    /// ��ȡһ�����͵Ĺ�������GameObject
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
    /// ���һ���µ� ��ֲ ������
    /// </summary>
    public void AddNewWorkSpace(string newWorkSpaceName, string cropName)
    {
        newWorkSpaceName = newWorkSpaceName + index++.ToString();
        if (TotalWorkSpaceDictionary.ContainsKey(newWorkSpaceName))
        {
            Debug.LogWarning("����������ͬ���ֵĹ�����");
            return;
        }

        Action<GameObject> onPlace = (GameObject obj) =>
        {
            if (obj.TryGetComponent<WorkSpace_Farm>(out WorkSpace_Farm workSpace_Farm))
            {
                workSpace_Farm.Init(cropName);
            }
        };

        // ��ʼ����
        StartCoroutine(AddWorkSpaceCo(newWorkSpaceName, onPlace));
    }

    public void LoadFarmWorkSpaceFromSave(Data_GameSave gameSave)
    {
        foreach(var fws in gameSave.SaveFarmWorkSpace)
        {
            // ������ֲ����
            Vector2 vector_bl = new Vector2(fws.bounds.min.x, fws.bounds.min.y);
            Vector2 vector_tr = new Vector2(fws.bounds.max.x, fws.bounds.max.y);
            GameObject go = WorkSpacePlacedFinally(fws.workSpaceName, vector_bl, vector_tr);
            WorkSpace_Farm wsf = go.GetComponent<WorkSpace_Farm>();
            wsf.Init(fws.cropName);
            // �����Ѿ���ֲ���������
            foreach(var cropSave in fws.crops)
            {
                wsf.SetAPositionHadFarmed(cropSave);
            }
        }

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

    private IEnumerator AddWorkSpaceCo(string newWorkSpaceName, Action<GameObject> onComplete)
    {
        IsDoingWorkSpace = true;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseDownPosition = mousePosition;

        bool flag = false;
        while (IsDoingWorkSpace)
        {
            // �������λ��
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // ����갴��ʱ��׼�������¹�������
            if (Input.GetMouseButtonDown(0))
            {
                mouseDownPosition = mousePosition;
                flag = true;
            }

            Vector2 pointA = mousePosition;
            Vector2 pointB = flag ? mouseDownPosition : mousePosition;

            // ������εı߽�
            float minX = Mathf.Floor(Mathf.Min(pointA.x, pointB.x));
            float maxX = Mathf.Ceil(Mathf.Max(pointA.x, pointB.x));
            float minY = Mathf.Floor(Mathf.Min(pointA.y, pointB.y));
            float maxY = Mathf.Ceil(Mathf.Max(pointA.y, pointB.y));

            // ȷ�����εĿ�Ⱥ͸߶�����Ϊ1
            if (maxX - minX < 1)
            {
                maxX = minX + 1;
            }
            if (maxY - minY < 1)
            {
                maxY = minY + 1;
            }

            // ������ε��ĸ�����
            Vector2 topLeft = new Vector2(minX, maxY);
            Vector2 topRight = new Vector2(maxX, maxY);
            Vector2 bottomLeft = new Vector2(minX, minY);
            Vector2 bottomRight = new Vector2(maxX, minY);

            // ���ö���λ��
            Vector3[] corners = new Vector3[]
            {
            new Vector3(topLeft.x, topLeft.y, 0),
            new Vector3(topRight.x, topRight.y, 0),
            new Vector3(bottomRight.x, bottomRight.y, 0),
            new Vector3(bottomLeft.x, bottomLeft.y, 0),
            new Vector3(topLeft.x, topLeft.y, 0) // �ص����
            };

            // �����δ����ʱ
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

            // ������������ʱ����鵱ǰλ���Ƿ����Ҫ�󣬲���ʾ
            if (Input.GetMouseButton(0) && flag)
            {
                // ������갴�µ�λ�ú͵�ǰ��λ�û��Ʒ���
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
            // ������ɿ�ʱ��ȷ�Ϸ���
            if (Input.GetMouseButtonUp(0) && flag)
            {
                if (IsOk(mouseDownPosition, mouseDownPosition))
                {
                    GameObject obj = WorkSpacePlacedFinally(newWorkSpaceName, bottomLeft, topRight);
                    // ����LineRenderer
                    ResetLineRenderer();
                    IsDoingWorkSpace = false;

                    onComplete?.Invoke(obj);
                }
                else
                {
                    Cancel();
                }
            }
            // ������Ҽ����£�ȡ�����ι���������
            if (Input.GetMouseButtonDown(1))
            {
                Cancel();
                break;
            }
            yield return null;
        }

        bool IsOk(Vector2 bl, Vector2 tr)
        {
            // ������η�Χ, �޳�����
            Vector2 pa = bl;
            pa.x += 0.05f;
            pa.y += 0.05f;
            Vector2 pb = tr;
            pb.x += -0.05f;
            pb.y += -0.05f;
            Collider2D[] hitColliders = Physics2D.OverlapAreaAll(pa, pb);
            foreach (Collider2D collider in hitColliders)
            {
                // ���κ�һ����ײ��ͷ���
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
            // �����ߵĶ���
            lineRenderer.positionCount = 5;
            lineRenderer.SetPositions(corners);
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

        void Cancel()
        {
            ResetLineRenderer();
            index--;
            IsDoingWorkSpace = false;
        }
    }

    private GameObject WorkSpacePlacedFinally(string newWorkSpaceName, Vector2 vector_bl, Vector2 vector_tr)
    {
        // ����ʵ��
        GameObject obj = Instantiate(WorkSpacePrefab, transform);
        obj.name = newWorkSpaceName;
        obj.tag = "WorkSpace";
        WorkSpace workSpace = obj.GetComponent<WorkSpace>();

        // ����sr�Ĵ�СΪ�����С
        workSpace.SetSize(vector_tr, vector_bl);
        workSpace.gameObject.SetActive(true);

        TotalWorkSpaceDictionary.Add(newWorkSpaceName, workSpace);
        return obj;
    }
}
