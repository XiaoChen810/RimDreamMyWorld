using ChenChen_MapGenerator;
using ChenChen_Scene;
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
            LoadAllSave();
            UITool.TryGetChildComponentByName<Button>("BtnNew").onClick.AddListener(() =>
            {
                SceneSystem.Instance.SetScene(new InitScene());
            });
            UITool.TryGetChildComponentByName<Button>("BtnDelete").onClick.AddListener(() =>
            {
                PlayManager.Instance.Delete(selectedGameSave);
                selectedGameSave = null;
                LoadAllSave();
            });
            UITool.TryGetChildComponentByName<Button>("BtnContinue").onClick.AddListener(() =>
            {
                if (selectedGameSave != null)
                {
                    PanelManager.RemovePanel(this);
                    PlayManager.Instance.Load(selectedGameSave);
                    SceneSystem.Instance.SetScene(new MainScene(() =>
                    {
                        MapManager.Instance.LoadOrGenerateSceneMap(selectedGameSave.SaveMap.mapName);
                    }));
                }
            });         
        }

        private void LoadAllSave()
        {
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
            // 先清空内容
            for (int i = 0; i < content.transform.childCount; i++)
            {
                GameObject.Destroy(content.transform.GetChild(i).gameObject);
            }
            // 加载所有存档
            foreach (var save in PlayManager.Instance.SaveList)
            {
                GameObject saveInstance = Object.Instantiate(savePrefab);
                saveInstance.transform.Find("TextName").GetComponent<Text>().text = $"Name: {save.SaveName}";
                saveInstance.transform.Find("TextDate").GetComponent<Text>().text = $"Date: {save.SaveDate}";
                saveInstance.transform.Find("TextSeed").GetComponent<Text>().text = $"Seed: {save.SaveMap.seed}";
                saveInstance.GetComponent<SaveDefaultPanel>().Data_GameSave = save;
                saveInstance.transform.SetParent(content.transform, false);
            }
            // 获取内容中的全部按钮添加功能
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