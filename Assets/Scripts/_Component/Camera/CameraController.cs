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
        // ��ȡ������ĵ�ǰ����ֵ
        float currentZoom = Camera.main.orthographicSize;

        // ������������
        float zoomDelta = -scrollInput * zoomSpeed;

        // ����������������ֵ
        float newZoom = currentZoom + zoomDelta;

        // ��������ֵ��Ԥ����ķ�Χ��
        newZoom = Mathf.Clamp(newZoom, zoomMin, zoomMax);

        // �������������ֵӦ�õ����������Ұ��
        Camera.main.orthographicSize = newZoom;
    }
}
