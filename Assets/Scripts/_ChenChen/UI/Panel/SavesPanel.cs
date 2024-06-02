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

        private Data_GameSave selectedGameSave;

        public override void OnEnter()
        {
            selectedGameSave = PlayManager.Instance.CurSave;
            // �ж����޴浵
            if(selectedGameSave != null)
            {
                UITool.TryGetChildComponentByName<Text>("��ʾ").text = "�Ƿ������Ϸ��";
            }
            else
            {
                UITool.TryGetChildComponentByName<Text>("��ʾ").text = "�޴浵\n���½���Ϸ";
            }

            // �¿�ʼ
            UITool.TryGetChildComponentByName<Button>("BtnNew").onClick.AddListener(() =>
            {
                Debug.Log("New Game");
                SceneSystem.Instance.SetScene(new InitScene());
            });
            // ������Ϸ
            UITool.TryGetChildComponentByName<Button>("BtnContinue").onClick.AddListener(() =>
            {
                if (selectedGameSave != null)
                {
                    PanelManager.RemoveTopPanel(this);
                    Action onPreloadAnimation = () =>
                    {
                        Debug.Log("Continue Game");
                    };
                    Action onPostLoadScene = () =>
                    {
                        // ������Ϸ�浵
                        PlayManager.Instance.Load();
                        // �򿪸ոռ��صĴ浵�ĵ�ͼ
                        MapManager.Instance.LoadOrGenerateSceneMap(selectedGameSave.SaveMap.mapName);
                    };
                    SceneSystem.Instance.SetScene(new MainScene(onPreloadAnimation, onPostLoadScene, 1f));
                }
                else
                {
                    UITool.TryGetChildComponentByName<Text>("��ʾ").text = "�޴浵\n�����½�";
                }

            });
            // �رմ���
            UITool.TryGetChildComponentByName<Button>("CloseBtn").onClick.AddListener(() =>
            {
                PanelManager.RemoveTopPanel(this);
            });
        }
    }
}