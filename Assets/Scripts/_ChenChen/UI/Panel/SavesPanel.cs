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
            // �ж��Ƿ����´浵
            if(curSave.IsNew)
            {
                UITool.TryGetChildComponentByName<Text>("��ʾ").text = "�޴浵\n���½���Ϸ";
            }
            else
            {
                UITool.TryGetChildComponentByName<Text>("��ʾ").text = "�Ƿ������Ϸ��";
            }

            // �¿�ʼ
            UITool.TryGetChildComponentByName<Button>("BtnNew").onClick.AddListener(() =>
            {
                Debug.Log("New Game");
                PlayManager.Instance.Delete();
                SceneSystem.Instance.SetScene(new InitScene());
            });
            // ������Ϸ
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
                        // ������Ϸ�浵
                        PlayManager.Instance.Load();
                        // �򿪸ոռ��صĴ浵�ĵ�ͼ
                        MapManager.Instance.LoadOrGenerateSceneMap(curSave.SaveMap.mapName);
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