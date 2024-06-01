using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Texture2D defaultCursor;
    public Texture2D clickCursor;
    public float cursorScale = 1.0f;

    private void Start()
    {
        // ����Ĭ�Ϲ��
        SetCursor(defaultCursor);
    }

    private void Update()
    {
        // ������������ʱ���Ĺ��
        if (Input.GetMouseButtonDown(0))
        {
            SetCursor(clickCursor);
        }

        // ���������ɿ�ʱ�ָ�Ĭ�Ϲ��
        if (Input.GetMouseButtonUp(0))
        {
            SetCursor(defaultCursor);
        }
    }

    private void SetCursor(Texture2D cursorTexture)
    {
        // ���Ź��
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        Cursor.visible = true; // ȷ�����ɼ�
        Cursor.lockState = CursorLockMode.None; // �ͷŹ������״̬
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);

        // Ӧ������
        Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.width * cursorScale, cursorTexture.height * cursorScale), CursorMode.Auto);
    }
}
