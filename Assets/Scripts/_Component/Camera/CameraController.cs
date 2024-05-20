using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float Speed;

    public float zoomMin = 48;
    public float zoomMax = 192;
    [Range(1, 50)] public int zoomSpeed = 5;

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if(horizontal != 0f || vertical != 0f)
        {
            transform.position += new Vector3(horizontal, vertical, 0) * Speed * Time.deltaTime;
        }

        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");

        ZoomCamera(scrollWheelInput);
    }

    void ZoomCamera(float scrollInput)
    {
        // 获取摄像机的当前缩放值
        float currentZoom = Camera.main.orthographicSize;

        // 计算缩放增量
        float zoomDelta = -scrollInput * zoomSpeed;

        // 根据增量调整缩放值
        float newZoom = currentZoom + zoomDelta;

        // 限制缩放值在预定义的范围内
        newZoom = Mathf.Clamp(newZoom, zoomMin, zoomMax);

        // 将调整后的缩放值应用到摄像机的视野中
        Camera.main.orthographicSize = newZoom;
    }
}
