using ChenChen_UI;
using UnityEngine;

namespace ChenChen_Thing
{
    public class Thing_ToolTable : Thing_Furniture
    {
        public bool IsSuccess { get; private set; }

        public TargetPtr wookPos;

        private void Update()
        {
            // 检测鼠标点击事件
            if (Input.GetMouseButtonDown(0) && !ThingSystemManager.Instance.Tool.OnBuildMode)
            {
                Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D[] hitColliders = Physics2D.OverlapPointAll(worldPosition);
                foreach (Collider2D collider in hitColliders)
                {
                    if (collider == ColliderSelf)
                    {
                        OnClick();
                    }
                }
            }
        }

        // 被点击时弹出制作菜单
        public void OnClick()
        {
            //PanelManager.Instance.AddPanel(new ToolTablePanel(), false);
        }
    }
}
