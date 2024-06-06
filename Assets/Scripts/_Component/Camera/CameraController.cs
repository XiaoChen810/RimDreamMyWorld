using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("�ƶ���ʽ")]
    public bool UseKeyboard = true; // ����ASWD
    public bool UseEdge = true;     // ��Ե�ƶ�
    public bool UseMouse = true;    // ����м�

    public float moveSpeed = 10f; // ������ƶ��ٶ�
    public float edgeScrollThreshold = 10f; // ��굽��Ļ��Ե�ľ��룬��λ������

    public float zoomMin = 5; // ���ŵ���Сֵ
    public float zoomMax = 20; // ���ŵ����ֵ
    [Range(1, 50)] public int zoomSpeed = 10; // �����ٶȣ���ΧΪ1��50

    public Vector2 moveBoundsMin; // �ƶ��߽����Сֵ
    public Vector2 moveBoundsMax; // �ƶ��߽�����ֵ

    private Vector3 lastMousePosition; // ��һ�����λ��

    private void Update()
    {
        HandleZoom(); // ��������
        if (UseKeyboard) HandleKeyboardMovement(); // �������������ƶ�
        if (UseEdge) HandleEdgeScrolling(); // �����Ե�������ƶ�
        if (UseMouse) HandleMouseDrag(); // ��������м��϶�
    }

    // �������ŵķ���
    private void HandleZoom()
    {
        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheelInput != 0f)
        {
            ZoomCamera(scrollWheelInput);
        }
    }

    // ������������ƶ��ķ���
    private void HandleKeyboardMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0f || vertical != 0f)
        {
            Vector3 moveDirection = new Vector3(horizontal, vertical, 0).normalized;
            Vector3 newPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;
            transform.position = ApplyBounds(newPosition);
        }
    }

    // �����Ե�����ķ���
    private void HandleEdgeScrolling()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 moveDirection = Vector3.zero;

        if (mousePosition.x >= Screen.width - edgeScrollThreshold)
        {
            moveDirection += Vector3.right;
        }
        else if (mousePosition.x <= edgeScrollThreshold)
        {
            moveDirection += Vector3.left;
        }

        if (mousePosition.y >= Screen.height - edgeScrollThreshold)
        {
            moveDirection += Vector3.up;
        }
        else if (mousePosition.y <= edgeScrollThreshold)
        {
            moveDirection += Vector3.down;
        }

        if (moveDirection != Vector3.zero)
        {
            Vector3 newPosition = transform.position + moveDirection.normalized * moveSpeed * Time.deltaTime;
            transform.position = ApplyBounds(newPosition);
        }
    }

    // ��������м��϶��ķ���
    private void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(2)) // ����м�����
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(2)) // ����м���ס
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            Vector3 moveDirection = new Vector3(-delta.x, -delta.y, 0) * moveSpeed * 0.2f * Time.deltaTime;
            Vector3 newPosition = transform.position + moveDirection;
            transform.position = ApplyBounds(newPosition);
            lastMousePosition = Input.mousePosition;
        }
    }

    // ����������ķ���
    void ZoomCamera(float scrollInput)
    {
        float currentZoom = Camera.main.orthographicSize;
        float zoomDelta = -scrollInput * zoomSpeed;
        float newZoom = Mathf.Lerp(currentZoom, currentZoom + zoomDelta, Time.deltaTime * zoomSpeed);
        newZoom = Mathf.Clamp(newZoom, zoomMin, zoomMax);
        Camera.main.orthographicSize = newZoom;
    }

    // Ӧ�ñ߽����Ƶķ���
    private Vector3 ApplyBounds(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, moveBoundsMin.x, moveBoundsMax.x);
        position.y = Mathf.Clamp(position.y, moveBoundsMin.y, moveBoundsMax.y);
        return position;
    }
}
