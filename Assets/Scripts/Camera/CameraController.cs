using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    private PixelPerfectCamera PixelPerfectCamera;
    public float Speed;
    public float zoomSpeed = 5f; // 调整缩放速度


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

        // 获取鼠标滚轮输入
        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");

        // 调整镜头远近
        ZoomCamera(scrollWheelInput);
    }

    void ZoomCamera(float scrollInput)
    {
        // 获取当前镜头的orthographicSize
        float currentSize = PixelPerfectCamera.assetsPPU;

        // 根据滚轮输入调整镜头远近
        currentSize -= scrollInput * zoomSpeed * Time.deltaTime;

        // 设置新的PixelPerfectCamera的assetsPPU，确保不小于某个最小值
        currentSize = Mathf.Max(currentSize, 1f);

        // 设置新的PixelPerfectCamera的assetsPPU
        PixelPerfectCamera.assetsPPU = Mathf.RoundToInt(currentSize);

    }
}
