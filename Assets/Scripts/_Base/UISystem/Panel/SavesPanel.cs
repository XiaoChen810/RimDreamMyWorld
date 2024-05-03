using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UISystem
{
    public class SavesPanel : PanelBase
    {
        public static readonly string path = "UI/Panel/Menus/SavesPanel";
        public SavesPanel() : base(new UIType(path))
        {
        }

        private Data_GameSave selectedGameSave;

        public override void OnEnter()
        {
            UITool.TryGetChildComponentByName<Button>("BtnNew").onClick.AddListener(() =>
            {

            });
            UITool.TryGetChildComponentByName<Button>("BtnContinue").onClick.AddListener(() =>
            {
                if(selectedGameSave != null)
                {
                    PanelManager.Instance.RemovePanel(this);
                    PlayManager.Instance.Load(selectedGameSave);
                }
            });
            // 获取装内容的子物体
            GameObject content = UITool.GetChildByName("Content");
            // 检查是否有GridLayoutGroup组件
            GridLayoutGroup glg = UITool.TryGetChildComponentByName<GridLayoutGroup>("Content");
            // 获取存档的预制件
            string saveDefaultPath = "UI/Component/SaveDefault";
            GameObject savePrefab = Resources.Load(saveDefaultPath) as GameObject;
            if (savePrefab == null)
            {
                Debug.LogError("存档的预制件为空, 检查位置: " + saveDefaultPath);
                PanelManager.RemovePanel(this);
                return;
            }
            // 加载所有存档
            foreach (var save in PlayManager.Instance.SaveList)
            {
                GameObject saveInstance = Object.Instantiate(savePrefab);
                saveInstance.transform.Find("TextName").GetComponent<Text>().text = $"Name: {save.SaveName}";
                saveInstance.transform.Find("TextDate").GetComponent<Text>().text = $"Name: {save.SaveDate}";
                saveInstance.transform.Find("TextSeed").GetComponent<Text>().text = $"Name: {save.SaveMap.seed}";
                saveInstance.GetComponent<SaveDefaultPanel>().Data_GameSave = save; 
                saveInstance.transform.SetParent(content.transform, false);
            }
            // 获取内容中的全部子物体
            Button[] btnContents = UITool.GetChildByName("Content").GetComponentsInChildren<Button>(true);
            foreach (var btn in btnContents)
            {
                btn.onClick.AddListener(() =>
                {
                    selectedGameSave = btn.GetComponent<SaveDefaultPanel>().Data_GameSave;
                });
            }
        }
    }
}