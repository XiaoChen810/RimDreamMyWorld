using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class CameraController : MonoBehaviour
{
    private PixelPerfectCamera PixelPerfectCamera;
    public float Speed;
    public float zoomSpeed = 5f; // ���������ٶ�


    private void Start()
    {
        PixelPerfectCamera = GetComponent<PixelPerfectCamera>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if(horizontal != 0f || vertical != 0f)
        {
            transform.position += new Vector3(horizontal, vertical, 0) * Speed * Time.deltaTime;
        }

        // ��ȡ����������
        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");

        // ������ͷԶ��
        ZoomCamera(scrollWheelInput);
    }

    void ZoomCamera(float scrollInput)
    {
        // ��ȡ��ǰ��ͷ��orthographicSize
        float currentSize = PixelPerfectCamera.assetsPPU;

        // ���ݹ������������ͷԶ��
        currentSize -= scrollInput * zoomSpeed * Time.deltaTime;

        // �����µ�PixelPerfectCamera��assetsPPU��ȷ����С��ĳ����Сֵ
        currentSize = Mathf.Max(currentSize, 1f);

        // �����µ�PixelPerfectCamera��assetsPPU
        PixelPerfectCamera.assetsPPU = Mathf.RoundToInt(currentSize);

    }
}
