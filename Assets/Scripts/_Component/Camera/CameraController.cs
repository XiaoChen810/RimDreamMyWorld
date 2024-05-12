using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class CameraController : MonoBehaviour
{
    private PixelPerfectCamera PixelPerfectCamera;
    public float Speed;

    public float zoomMin = 48;
    public float zoomMax = 192;
    [Range(10, 50)] public int zoomSpeed = 5;

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

        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");

        ZoomCamera(scrollWheelInput);
    }

    void ZoomCamera(float scrollInput)
    {
        float currentSize = PixelPerfectCamera.assetsPPU;

        currentSize -= scrollInput * zoomSpeed;

        currentSize = Mathf.Max(currentSize, zoomMin);
        currentSize = Mathf.Min(currentSize, zoomMax);

        PixelPerfectCamera.assetsPPU = Mathf.RoundToInt(currentSize);
    }
}
