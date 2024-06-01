using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Texture2D defaultCursor;
    public Texture2D clickCursor;
    public float cursorScale = 1.0f;

    private void Start()
    {
        // 设置默认光标
        SetCursor(defaultCursor);
    }

    private void Update()
    {
        // 当鼠标左键按下时更改光标
        if (Input.GetMouseButtonDown(0))
        {
            SetCursor(clickCursor);
        }

        // 当鼠标左键松开时恢复默认光标
        if (Input.GetMouseButtonUp(0))
        {
            SetCursor(defaultCursor);
        }
    }

    private void SetCursor(Texture2D cursorTexture)
    {
        // 缩放光标
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        Cursor.visible = true; // 确保光标可见
        Cursor.lockState = CursorLockMode.None; // 释放光标锁定状态
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);

        // 应用缩放
        Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.width * cursorScale, cursorTexture.height * cursorScale), CursorMode.Auto);
    }
}
