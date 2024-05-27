using ChenChen_UI;
using UnityEngine;

namespace ChenChen_Thing
{
    public class Thing_ToolTable : Thing_Building
    {
        public bool IsSuccess { get; private set; }

        public TargetPtr wookPos;

        private void Update()
        {
            // 检测鼠标点击事件
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform == this.transform)
                    {
                        OnClick();
                    }
                }
            }
        }

        // 被点击时弹出制作菜单
        public void OnClick()
        {
            PanelManager.Instance.AddPanel(new ToolTablePanel(), false);
        }
    }
}
