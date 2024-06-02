using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float Speed; // ������ƶ��ٶ�
    public float edgeScrollSpeed = 10f; // ��Ե�����ٶ�
    public float edgeScrollThreshold = 10f; // ��굽��Ļ��Ե�ľ��룬��λ������

    public float zoomMin = 48; // ���ŵ���Сֵ
    public float zoomMax = 192; // ���ŵ����ֵ
    [Range(1, 50)] public int zoomSpeed = 5; // �����ٶȣ���ΧΪ1��50

    public Vector2 moveBoundsMin; // �ƶ��߽����Сֵ
    public Vector2 moveBoundsMax; // �ƶ��߽�����ֵ

    private void Update()
    {
        HandleKeyboardMovement(); // �������������ƶ�
        HandleEdgeScrolling(); // �����Ե�������ƶ�
        HandleZoom(); // ��������
    }

    // ������������ƶ��ķ���
    private void HandleKeyboardMovement()
    {
        // ��ȡˮƽ�ʹ�ֱ�������ֵ
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // �����ˮƽ��ֱ���룬���ƶ������
        if (horizontal != 0f || vertical != 0f)
        {
            // �����ƶ����򲢹�һ��
            Vector3 moveDirection = new Vector3(horizontal, vertical, 0).normalized;
            // �����ٶȺ�ʱ���ƶ����������Ӧ�ñ߽�����
            Vector3 newPosition = transform.position + moveDirection * Speed * Time.deltaTime;
            transform.position = ApplyBounds(newPosition);
        }
    }

    // �����Ե�����ķ���
    private void HandleEdgeScrolling()
    {
        // ��ȡ��굱ǰλ��
        Vector3 mousePosition = Input.mousePosition;
        Vector3 moveDirection = Vector3.zero;

        // �������Ƿ�����Ļ�ұ�Ե
        if (mousePosition.x >= Screen.width - edgeScrollThreshold)
        {
            moveDirection += Vector3.right;
        }
        // �������Ƿ�����Ļ���Ե
        else if (mousePosition.x <= edgeScrollThreshold)
        {
            moveDirection += Vector3.left;
        }

        // �������Ƿ�����Ļ�ϱ�Ե
        if (mousePosition.y >= Screen.height - edgeScrollThreshold)
        {
            moveDirection += Vector3.up;
        }
        // �������Ƿ�����Ļ�±�Ե
        else if (mousePosition.y <= edgeScrollThreshold)
        {
            moveDirection += Vector3.down;
        }

        // ����б�Ե�������ƶ��������ƶ����������Ӧ�ñ߽�����
        if (moveDirection != Vector3.zero)
        {
            Vector3 newPosition = transform.position + moveDirection.normalized * edgeScrollSpeed * Time.deltaTime;
            transform.position = ApplyBounds(newPosition);
        }
    }

    // �������ŵķ���
    private void HandleZoom()
    {
        // ��ȡ�����ֵ�����ֵ
        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");
        // ����й������룬����ZoomCamera������������
        if (scrollWheelInput != 0f)
        {
            ZoomCamera(scrollWheelInput);
        }
    }

    // ����������ķ���
    void ZoomCamera(float scrollInput)
    {
        // ��ȡ��ǰ�����������ֵ
        float currentZoom = Camera.main.orthographicSize;
        // ������������
        float zoomDelta = -scrollInput * zoomSpeed;
        // ʹ��Mathf.Lerpƽ���ؼ����µ�����ֵ
        float newZoom = Mathf.Lerp(currentZoom, currentZoom + zoomDelta, Time.deltaTime * zoomSpeed);
        // ���µ�����ֵ������Ԥ����ķ�Χ��
        newZoom = Mathf.Clamp(newZoom, zoomMin, zoomMax);
        // �������������ֵӦ�õ������
        Camera.main.orthographicSize = newZoom;
    }

    // Ӧ�ñ߽����Ƶķ���
    private Vector3 ApplyBounds(Vector3 position)
    {
        // ����X��Y���λ���ڶ������Сֵ�����ֵ֮��
        position.x = Mathf.Clamp(position.x, moveBoundsMin.x, moveBoundsMax.x);
        position.y = Mathf.Clamp(position.y, moveBoundsMin.y, moveBoundsMax.y);
        return position;
    }
}
