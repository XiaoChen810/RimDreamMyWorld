using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float Speed; // 摄像机移动速度
    public float edgeScrollSpeed = 10f; // 边缘滚动速度
    public float edgeScrollThreshold = 10f; // 鼠标到屏幕边缘的距离，单位是像素

    public float zoomMin = 48; // 缩放的最小值
    public float zoomMax = 192; // 缩放的最大值
    [Range(1, 50)] public int zoomSpeed = 5; // 缩放速度，范围为1到50

    public Vector2 moveBoundsMin; // 移动边界的最小值
    public Vector2 moveBoundsMax; // 移动边界的最大值

    private void Update()
    {
        HandleKeyboardMovement(); // 处理键盘输入的移动
        HandleEdgeScrolling(); // 处理边缘滚动的移动
        HandleZoom(); // 处理缩放
    }

    // 处理键盘输入移动的方法
    private void HandleKeyboardMovement()
    {
        // 获取水平和垂直输入轴的值
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 如果有水平或垂直输入，则移动摄像机
        if (horizontal != 0f || vertical != 0f)
        {
            // 计算移动方向并归一化
            Vector3 moveDirection = new Vector3(horizontal, vertical, 0).normalized;
            // 根据速度和时间移动摄像机，并应用边界限制
            Vector3 newPosition = transform.position + moveDirection * Speed * Time.deltaTime;
            transform.position = ApplyBounds(newPosition);
        }
    }

    // 处理边缘滚动的方法
    private void HandleEdgeScrolling()
    {
        // 获取鼠标当前位置
        Vector3 mousePosition = Input.mousePosition;
        Vector3 moveDirection = Vector3.zero;

        // 检查鼠标是否在屏幕右边缘
        if (mousePosition.x >= Screen.width - edgeScrollThreshold)
        {
            moveDirection += Vector3.right;
        }
        // 检查鼠标是否在屏幕左边缘
        else if (mousePosition.x <= edgeScrollThreshold)
        {
            moveDirection += Vector3.left;
        }

        // 检查鼠标是否在屏幕上边缘
        if (mousePosition.y >= Screen.height - edgeScrollThreshold)
        {
            moveDirection += Vector3.up;
        }
        // 检查鼠标是否在屏幕下边缘
        else if (mousePosition.y <= edgeScrollThreshold)
        {
            moveDirection += Vector3.down;
        }

        // 如果有边缘滚动的移动方向，则移动摄像机，并应用边界限制
        if (moveDirection != Vector3.zero)
        {
            Vector3 newPosition = transform.position + moveDirection.normalized * edgeScrollSpeed * Time.deltaTime;
            transform.position = ApplyBounds(newPosition);
        }
    }

    // 处理缩放的方法
    private void HandleZoom()
    {
        // 获取鼠标滚轮的输入值
        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");
        // 如果有滚轮输入，调用ZoomCamera方法进行缩放
        if (scrollWheelInput != 0f)
        {
            ZoomCamera(scrollWheelInput);
        }
    }

    // 缩放摄像机的方法
    void ZoomCamera(float scrollInput)
    {
        // 获取当前的摄像机缩放值
        float currentZoom = Camera.main.orthographicSize;
        // 计算缩放增量
        float zoomDelta = -scrollInput * zoomSpeed;
        // 使用Mathf.Lerp平滑地计算新的缩放值
        float newZoom = Mathf.Lerp(currentZoom, currentZoom + zoomDelta, Time.deltaTime * zoomSpeed);
        // 将新的缩放值限制在预定义的范围内
        newZoom = Mathf.Clamp(newZoom, zoomMin, zoomMax);
        // 将调整后的缩放值应用到摄像机
        Camera.main.orthographicSize = newZoom;
    }

    // 应用边界限制的方法
    private Vector3 ApplyBounds(Vector3 position)
    {
        // 限制X和Y轴的位置在定义的最小值和最大值之间
        position.x = Mathf.Clamp(position.x, moveBoundsMin.x, moveBoundsMax.x);
        position.y = Mathf.Clamp(position.y, moveBoundsMin.y, moveBoundsMax.y);
        return position;
    }
}
