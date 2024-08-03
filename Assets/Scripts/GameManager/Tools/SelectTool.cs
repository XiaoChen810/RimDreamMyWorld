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
        // ����ڽ���ģʽ���򲻼������
        if (ThingSystemManager.Instance.Tool.OnBuildMode) return;
        // ������ڷ��ù������򲻼������
        if (GameManager.Instance.WorkSpaceTool.IsDoingWorkSpace) return;
        InputUpdate();
    }

    private void InputUpdate()
    {
        if (UnityEngine.Input.GetMouseButtonDown(0))
        {
            // �������Ƿ���UI��
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            // ��¼������ʼ��λ��
            startPos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            isDragging = true;
        }

        if (UnityEngine.Input.GetMouseButton(0))
        {
            if (!isDragging) return;

            // ���»���������λ��
            endPos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);

            // ����������볬��һ��ֵ�ͻ��Ʒ���
            if (Vector2.Distance(startPos, endPos) > 0.1f)
            {
                DrawLineBox(startPos, endPos);
            }
        }

        if (UnityEngine.Input.GetMouseButtonUp(0))
        {
            if (!isDragging) return;

            // �ڻ�������ʱ�����ѡ
            HandleSelection(startPos, endPos);
            ResetLineRenderer();
            isDragging = false;
        }
    }

    private void InitializeLineRenderer()
    {
        // �����ߵĿ��
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    private void DrawLineBox(Vector2 start, Vector2 end)
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

    private void ResetLineRenderer()
    {
        // �����߿�
        lineRenderer.positionCount = 0;
    }

    private List<DetailView> dvs = new();
    private void HandleSelection(Vector2 start, Vector2 end)
    {
        // �����ﴦ���ѡ�߼�
        Collider2D[] hitColliders = Physics2D.OverlapAreaAll(start, end);

        // ִ��ѡ���߼�, �κ�һ���߼��ɹ����ͷ��أ�˳��Ϊ���ȼ�
        foreach(var d in dvs)
        {
            //�ر���һ��DetailView��ѡ��
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

