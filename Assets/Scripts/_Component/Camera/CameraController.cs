using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [Header("�ƶ���ʽ")]
    public bool UseKeyboard = true; // ����ASWD
    public bool UseEdge = true;     // ��Ե�ƶ�
    public bool UseMouse = true;    // ����м�

    [Header("Speed")]
    public float speedMin = 10; // �ٶȵ���Сֵ
    public float speedMax = 30; // �ٶȵ����ֵ
    [SerializeField] private float moveSpeed = 10f; // ������ƶ��ٶ�
    public float MoveSpeed
    {
        get
        {
            return moveSpeed;
        }
        set
        {
            moveSpeed = Mathf.Clamp(value, speedMin, speedMax);
        }
    }

    [Header("Zoom")]
    public float zoomMin = 5; // ���ŵ���Сֵ
    public float zoomMax = 20; // ���ŵ����ֵ
    public float zoomSpeedMin = 20; // ���ŵ���Сֵ
    public float zoomSpeedMax = 50; // ���ŵ����ֵ
    [SerializeField] private float zoomSpeed = 30; // �����ٶ�
    public float ZoomSpeed
    {
        get
        {
            return zoomSpeed;
        }
        set
        {
            zoomSpeed = Mathf.Clamp(value, zoomSpeedMin, zoomSpeedMax);
        }
    }

    [Header("Range")]
    public Vector2 moveBoundsMin = new Vector2(5, 5); // �ƶ��߽����Сֵ
    public Vector2 moveBoundsMax = new Vector2(245, 245); // �ƶ��߽�����ֵ
    public float edgeScrollThreshold = 10f; // ��굽��Ļ��Ե�ľ��룬��λ������

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
            Vector3 moveDirection = new Vector3(-delta.x, -delta.y, 0) * moveSpeed * Time.deltaTime;
            Vector3 newPosition = transform.position + moveDirection;
            transform.position = ApplyBounds(newPosition);
            lastMousePosition = Input.mousePosition;
        }
    }

    // ����������ķ���
    void ZoomCamera(float scrollInput)
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
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
