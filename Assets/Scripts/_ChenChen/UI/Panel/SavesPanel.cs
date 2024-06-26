using ChenChen_Map;
using ChenChen_Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class SavesPanel : PanelBase
    {
        public static readonly string path = "UI/Panel/Menus/SavesPanel";
        public SavesPanel() : base(new UIType(path))
        {
        }

        private Data_GameSave curSave;

        public override void OnEnter()
        {
            curSave = PlayManager.Instance.CurSave;
            // 判断是否是新存档
            if(curSave.IsNew)
            {
                UITool.TryGetChildComponentByName<Text>("提示").text = "无存档\n请新建游戏";
            }
            else
            {
                UITool.TryGetChildComponentByName<Text>("提示").text = "是否继续游戏？";
            }

            // 新开始
            UITool.TryGetChildComponentByName<Button>("BtnNew").onClick.AddListener(() =>
            {
                Debug.Log("New Game");
                PlayManager.Instance.Delete();
                SceneSystem.Instance.SetScene(new InitScene());
            });
            // 继续游戏
            UITool.TryGetChildComponentByName<Button>("BtnContinue").onClick.AddListener(() =>
            {
                if (!curSave.IsNew)
                {
                    PanelManager.RemoveTopPanel(this);
                    Action onPreloadAnimation = () =>
                    {
                        Debug.Log("Continue Game");
                    };
                    Action onPostLoadScene = () =>
                    {
                        // 加载游戏存档
                        PlayManager.Instance.Load();
                        // 打开刚刚加载的存档的地图
                        MapManager.Instance.LoadOrGenerateSceneMap(curSave.SaveMap.mapName);
                    };
                    SceneSystem.Instance.SetScene(new MainScene(onPreloadAnimation, onPostLoadScene, 1f));
                }
                else
                {
                    UITool.TryGetChildComponentByName<Text>("提示").text = "无存档\n请点击新建";
                }

            });
            // 关闭窗口
            UITool.TryGetChildComponentByName<Button>("CloseBtn").onClick.AddListener(() =>
            {
                PanelManager.RemoveTopPanel(this);
            });
        }
    }
}