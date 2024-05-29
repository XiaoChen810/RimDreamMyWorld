using ChenChen_Thing;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class ToolTablePanel : PanelBase
    {
        private static readonly string path = "UI/Panel/Menus/ToolTablePanel";

        public ToolTablePanel() : base(new UIType(path))
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            // 关闭菜单的按钮
            UITool.TryGetChildComponentByName<Button>("Btn关闭").onClick.AddListener(() =>
            {
                PanelManager.RemoveTopPanel(this);
            });

            // 加载所有能制作的东西
            InitContent();

            GameManager.Instance.PauseGame();
        }

        public override void OnExit()
        {
            base.OnExit();
            GameManager.Instance.RecoverGame();
        }

        private void InitContent()
        {
            GameObject content = UITool.GetChildByName("Content");
            foreach (Transform child in content.transform)
            {
                Object.Destroy(child);
            }

            string path = "UI/Component/StuffMaker";
            GameObject stuffMakerPrefab = Resources.Load(path) as GameObject;
            if (stuffMakerPrefab == null)
            {
                Debug.LogError("预制件为空, 检查位置: " + path);
                PanelManager.RemoveTopPanel(this);
                return;
            }

            foreach (var stuffdef in StorageManager.Instance.StuffDefDictionary)
            {
                bool flag = true;
                if (!stuffdef.Value.CanMake) flag = false;
                foreach (string nt in stuffdef.Value.needsTechs)
                {
                    // 有一个前置科技没解锁就下一个
                    if (!GameManager.Instance.TechnologyTool.IsUnlock(nt))
                    {
                        flag = false;
                        break;
                    }
                }

                if (flag)
                {
                    GameObject stuffMaker = Object.Instantiate(stuffMakerPrefab);
                    stuffMaker.name = $"StuffMaker";
                    stuffMaker.transform.Find("StuffName").GetComponent<Text>().text = stuffdef.Value.Name;
                    foreach (string need in stuffdef.Value.needs)
                    {
                        stuffMaker.transform.Find("Needs").GetComponent<Text>().text += need + ",";
                    }
                    stuffMaker.transform.SetParent(content.transform, false);
                }
            }
        }
    }
}