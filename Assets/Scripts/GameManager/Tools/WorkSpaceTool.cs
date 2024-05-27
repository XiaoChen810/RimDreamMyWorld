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

    private IEnumerator AddWorkSpaceCo(string newWorkSpaceName, Action<GameObject> onPlace)
    {
        IsDoingWorkSpace = true;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePositionCeil;
        Vector2 mousePositionFloor;
        Vector2 mouseDownPosition = mousePosition;

        bool flag = false;
        while (IsDoingWorkSpace)
        {
            // �������λ��
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePositionCeil = mousePosition;
            mousePositionCeil.x = Mathf.Ceil(mousePosition.x);
            mousePositionCeil.y = Mathf.Ceil(mousePosition.y);
            mousePositionFloor = mousePosition;
            mousePositionFloor.x = Mathf.Floor(mousePosition.x);
            mousePositionFloor.y = Mathf.Floor(mousePosition.y);
            if (!flag)
            {
                if (IsOk(mousePositionCeil, mousePositionFloor))
                {
                    ChangeColor(Color.green);
                }
                else
                {
                    ChangeColor(Color.red);
                }
                DrawLineBox(mousePositionCeil, mousePositionFloor);
            }
            // ����갴��ʱ��׼�������¹�������
            if (Input.GetMouseButtonDown(0))
            {
                mouseDownPosition = mousePositionCeil;
                flag = true;
            }
            // ������������ʱ����鵱ǰλ���Ƿ����Ҫ�󣬲���ʾ
            if (Input.GetMouseButton(0) && flag)
            {
                // ������갴�µ�λ�ú͵�ǰ��λ�û��Ʒ���
                if (IsOk(mouseDownPosition, mousePositionFloor))
                {
                    ChangeColor(Color.green);
                }
                else
                {
                    ChangeColor(Color.red);
                }
                DrawLineBox(mouseDownPosition, mousePositionFloor);
            }
            // ������ɿ�ʱ��ȷ�Ϸ���
            if (Input.GetMouseButtonUp(0) && flag)
            {
                if (IsOk(mouseDownPosition, mousePositionFloor))
                {
                    GameObject obj = WorkSpacePlacedFinally(newWorkSpaceName, mousePositionFloor, mouseDownPosition);
                    // ����LineRenderer
                    ResetLineRenderer();
                    IsDoingWorkSpace = false;

                    onPlace?.Invoke(obj);
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

        bool IsOk(Vector2 start, Vector2 end)
        {
            if (Mathf.Abs(start.x - end.x) < 1) return false;
            if (Mathf.Abs(start.y - end.y) < 1) return false;
            Vector2 pa = start;
            pa.x += (start.x < end.x) ? 0.05f : -0.05f;
            pa.y += (start.y < end.y) ? 0.05f : -0.05f;
            Vector2 pb = end;
            pb.x += (end.x < start.x) ? 0.05f : -0.05f;
            pb.y += (end.y < start.y) ? 0.05f : -0.05f;
            Collider2D[] hitColliders = Physics2D.OverlapAreaAll(pa, pb);
            foreach (Collider2D collider in hitColliders)
            {
                // ���κ�һ����ײ��ͷ���
                return false;
            }
            var nodes = MapManager.Instance.CurMapNodes;
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
