using ChenChen_Map;
using ChenChen_Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class Panel_Saves : PanelBase
    {
        public static readonly string path = "UI/Panel/Menus/SavesPanel";
        public Panel_Saves() : base(new UIType(path))
        {
        }

        private Data_GameSave curSave;

        public override void OnEnter()
        {
            curSave = PlayManager.Instance.CurSave;
            if(curSave.IsNew)
            {
                UITool.TryGetChildComponentByName<Text>("提示").text = "无存档\n请新建游戏";
            }
            else
            {
                UITool.TryGetChildComponentByName<Text>("提示").text = "是否继续游戏？";
            }
            UITool.TryGetChildComponentByName<Button>("BtnNew").onClick.AddListener(() =>
            {
                Debug.Log("New Game");
                PlayManager.Instance.Delete();
                SceneSystem.Instance.SetScene(new InitScene());
            });
            UITool.TryGetChildComponentByName<Button>("BtnContinue").onClick.AddListener(() =>
            {
                if (!curSave.IsNew)
                {
                    PanelManager.RemovePanel(this);
                    Action onPreloadAnimation = () =>
                    {
                        Debug.Log("Continue Game");
                    };
                    Action onPostLoadScene = () =>
                    {
                        PlayManager.Instance.Load();
                        MapManager.Instance.LoadOrGenerateSceneMap(curSave.SaveMap.mapName);
                    };
                    SceneSystem.Instance.SetScene(new MainScene(onPreloadAnimation, onPostLoadScene, 1f));
                }
                else
                {
                    UITool.TryGetChildComponentByName<Text>("提示").text = "无存档\n请点击新建";
                }

            });
            UITool.TryGetChildComponentByName<Button>("CloseBtn").onClick.AddListener(() =>
            {
                PanelManager.RemovePanel(this);
            });
        }
    }
}