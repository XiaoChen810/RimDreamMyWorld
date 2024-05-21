using ChenChen_BuildingSystem;
using ChenChen_CropSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UISystem
{
    public class CropWorkSpacePanel : PanelBase
    {
        private static readonly string path = "UI/Panel/Menus/CropWorkSpacePanel";

        public CropWorkSpacePanel() : base(new UIType(path))
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            InitContent();
            UITool.TryGetChildComponentByName<Button>("Btn关闭").onClick.AddListener(() =>
            {
                PanelManager.RemoveTopPanel(this);
            });
        }

        private void InitContent()
        {
            Dictionary<string, CropDef> dict = CropManager.Instance.CropDefDictionary;
            // 获取装内容的子物体
            GameObject content = UITool.GetChildByName("Content");
            // 检查是否有GridLayoutGroup组件
            GridLayoutGroup glg = UITool.TryGetChildComponentByName<GridLayoutGroup>("Content");
            // 获取按钮的预制件
            GameObject btnPrefab = Resources.Load("UI/Component/BtnBlueprintDefault") as GameObject;
            if (btnPrefab == null)
            {
                Debug.LogError("按钮的预制件为空, 检查位置: UI/Component/BtnBlueprintDefault");
                PanelManager.RemoveTopPanel(this);
                return;
            }
            // 根据蓝图字典,设置成对应的按钮添加到内容(content)中
            foreach (var item in dict)
            {
                GameObject btnInstance = Object.Instantiate(btnPrefab);
                btnInstance.name = $"CropWorkSpace{item.Value.CropName}";
                GameObject btnImage = btnInstance.transform.Find("Image").gameObject;
                btnImage.GetComponent<Image>().sprite = item.Value.CropIcon;
                btnInstance.transform.SetParent(content.transform, false);
            }
            // 获取内容中的全部子物体
            Button[] btnContents = UITool.GetChildByName("Content").GetComponentsInChildren<Button>(true);
            foreach (var btn in btnContents)
            {
                btn.onClick.AddListener(() =>
                {
                    string cropName = btn.name.Replace("CropWorkSpace", "");
                    GameManager.Instance.WorkSpaceTool.AddNewWorkSpace($"种植区", cropName);
                    PanelManager.RemovePanel(this);
                });
            }
        }
    }
}