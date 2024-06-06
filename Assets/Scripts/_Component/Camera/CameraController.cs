using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("移动方式")]
    public bool UseKeyboard = true; // 键盘ASWD
    public bool UseEdge = true;     // 边缘移动
    public bool UseMouse = true;    // 鼠标中键

    public float moveSpeed = 10f; // 摄像机移动速度
    public float edgeScrollThreshold = 10f; // 鼠标到屏幕边缘的距离，单位是像素

    public float zoomMin = 5; // 缩放的最小值
    public float zoomMax = 20; // 缩放的最大值
    [Range(1, 50)] public int zoomSpeed = 10; // 缩放速度，范围为1到50

    public Vector2 moveBoundsMin; // 移动边界的最小值
    public Vector2 moveBoundsMax; // 移动边界的最大值

    private Vector3 lastMousePosition; // 上一次鼠标位置

    private void Update()
    {
        HandleZoom(); // 处理缩放
        if (UseKeyboard) HandleKeyboardMovement(); // 处理键盘输入的移动
        if (UseEdge) HandleEdgeScrolling(); // 处理边缘滚动的移动
        if (UseMouse) HandleMouseDrag(); // 处理鼠标中键拖动
    }

    // 处理缩放的方法
    private void HandleZoom()
    {
        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheelInput != 0f)
        {
            ZoomCamera(scrollWheelInput);
        }
    }

    // 处理键盘输入移动的方法
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

    // 处理边缘滚动的方法
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

    // 处理鼠标中键拖动的方法
    private void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(2)) // 鼠标中键按下
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(2)) // 鼠标中键按住
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            Vector3 moveDirection = new Vector3(-delta.x, -delta.y, 0) * moveSpeed * 0.2f * Time.deltaTime;
            Vector3 newPosition = transform.position + moveDirection;
            transform.position = ApplyBounds(newPosition);
            lastMousePosition = Input.mousePosition;
        }
    }

    // 缩放摄像机的方法
    void ZoomCamera(float scrollInput)
    {
        float currentZoom = Camera.main.orthographicSize;
        float zoomDelta = -scrollInput * zoomSpeed;
        float newZoom = Mathf.Lerp(currentZoom, currentZoom + zoomDelta, Time.deltaTime * zoomSpeed);
        newZoom = Mathf.Clamp(newZoom, zoomMin, zoomMax);
        Camera.main.orthographicSize = newZoom;
    }

    // 应用边界限制的方法
    private Vector3 ApplyBounds(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, moveBoundsMin.x, moveBoundsMax.x);
        position.y = Mathf.Clamp(position.y, moveBoundsMin.y, moveBoundsMax.y);
        return position;
    }
}
