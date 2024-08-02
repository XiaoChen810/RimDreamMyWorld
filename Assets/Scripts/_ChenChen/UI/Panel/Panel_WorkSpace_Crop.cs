using ChenChen_Thing;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class Panel_WorkSpace_Crop : PanelBase
    {
        private static readonly string path = "UI/Panel/Menus/CropWorkSpacePanel";

        public Panel_WorkSpace_Crop() : base(new UIType(path))
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            InitContent();
            UITool.TryGetChildComponentByName<Button>("Btn关闭").onClick.AddListener(() =>
            {
                PanelManager.RemovePanel(this);
            });
        }

        private void InitContent()
        {
            Dictionary<string, CropDef> dict = CropManager.Instance.CropDefDictionary;
            GameObject content = UITool.GetChildByName("Content");
            GameObject btnPrefab = Resources.Load("UI/Component/BtnBlueprintDefault") as GameObject;
            if (btnPrefab == null)
            {
                Debug.LogError("按钮的预制件为空, 检查位置: UI/Component/BtnBlueprintDefault");
                PanelManager.RemovePanel(this);
                return;
            }
            foreach (var item in dict)
            {
                GameObject btnInstance = Object.Instantiate(btnPrefab);
                btnInstance.name = $"CropWorkSpace{item.Value.CropName}";
                btnInstance.transform.SetParent(content.transform, false);
                GameObject btnImage = btnInstance.transform.Find("Image").gameObject;
                btnImage.GetComponent<Image>().sprite = item.Value.CropIcon;               
            }
            Button[] btnContents = UITool.GetChildByName("Content").GetComponentsInChildren<Button>(true);
            foreach (var btn in btnContents)
            {
                btn.onClick.AddListener(() =>
                {
                    string cropName = btn.name.Replace("CropWorkSpace", "");
                    GameManager.Instance.WorkSpaceTool.AddNewWorkSpace(cropName);
                    PanelManager.RemovePanel(this);
                });
            }
        }
    }
}