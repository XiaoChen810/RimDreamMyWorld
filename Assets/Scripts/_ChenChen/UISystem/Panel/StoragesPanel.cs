﻿using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UISystem
{
    public class StoragesPanel : PanelBase
    {
        private static readonly string path = "UI/Panel/Menus/StoragesPanel";
        public StoragesPanel() : base(new UIType(path))
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            InitContent();
        }


        private void InitContent()
        {
            GameObject content = UITool.GetChildByName("Content");
            foreach (Transform child in content.transform)
            {
                Object.Destroy(child);
            }

            string stuffInfomationPrefabPath = "UI/Component/StuffInfomation";
            GameObject stuffInfomationPrefab = Resources.Load(stuffInfomationPrefabPath) as GameObject;
            if (stuffInfomationPrefab == null)
            {
                Debug.LogError("预制件为空, 检查位置: " + stuffInfomationPrefabPath);
                PanelManager.RemoveTopPanel(this);
                return;
            }

            foreach (var storage in StorageManager.Instance.StoragesDictionary)
            {
                GameObject stuffInfomation = Object.Instantiate(stuffInfomationPrefab);
                stuffInfomation.name = $"BtnBlueprint{storage.Value.Def.Name}";
                stuffInfomation.transform.Find("NameText").GetComponent<Text>().text = storage.Value.Def.Name;
                stuffInfomation.transform.Find("NumText").GetComponent<Text>().text = $"数量: {storage.Value.Num}";
                stuffInfomation.transform.SetParent(content.transform, false);
            }
        }
    }
}