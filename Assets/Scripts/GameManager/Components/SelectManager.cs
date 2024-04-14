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
            // ��¼������ʼ��λ��
            startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            // ���»���������λ��
            endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // ����������볬��һ��ֵ�ͻ��Ʒ���
            if (Vector2.Distance(startPos, endPos) > 0.1f)
            {
                DrawLineBox(startPos, endPos);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            // �ڻ�������ʱ�����ѡ
            HandleSelection(startPos, endPos);
            ResetLineRenderer();
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

    private void HandleSelection(Vector2 start, Vector2 end)    
    {
        // �����ﴦ���ѡ�߼�
        Collider2D[] hitColliders = Physics2D.OverlapAreaAll(start, end);

        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Pawn"))
            {
                // ִ��ѡ���߼�
                Pawn controller = collider.GetComponent<Pawn>();

                controller.TrySelect();           
            }
        }
    }
}

