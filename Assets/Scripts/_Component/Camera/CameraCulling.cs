using ChenChen_BuildingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCulling : MonoBehaviour
{
    private Camera cam;
    private Quadtree quadtree;
    private Vector3 lastCameraPositon;
    public float FlashDistance = 1.0f;
    public float PreLoadRange = 5.0f;

    // 剔除视野外的物体
    private void Update()
    {
        if (!GameManager.Instance.GameIsStart) return;
        // 初始化
        if (cam == null || quadtree == null)
        {
            cam = Camera.main;
            quadtree = ThingSystemManager.Instance.Quadtree;
            lastCameraPositon = cam.transform.position;
            Flash();
        }
        if(Vector2.Distance(lastCameraPositon, cam.transform.position) >= FlashDistance)
        {
            lastCameraPositon = cam.transform.position;
            Flash();
        }
    }

    private void Flash()
    {
        // 计算摄像机的边界
        Rect cameraBounds = new Rect(
            cam.transform.position.x - cam.orthographicSize * cam.aspect - PreLoadRange,
            cam.transform.position.y - cam.orthographicSize - PreLoadRange,
            cam.orthographicSize * 2 * cam.aspect + 2 * PreLoadRange,
            cam.orthographicSize * 2 + 2 * PreLoadRange
        );

        // 从四叉树中检索摄像机视野内的对象
        quadtree.Retrieve(cameraBounds);
    }
}
