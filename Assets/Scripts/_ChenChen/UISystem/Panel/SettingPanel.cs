﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChenChen_Scene;
using ChenChen_MapGenerator;

namespace ChenChen_UISystem
{
    public class SettingPanel : PanelBase
    {
        static readonly string path = "UI/Panel/Menus/SettingPanel";
        public SettingPanel() : base(new UIType(path)) { }

        public override void OnEnter()
        {
            UITool.TryGetChildComponentByName<Button>("SaveBtn").onClick.AddListener(() =>
            {
                PlayManager.Instance.Save(data_MapSave: MapManager.Instance.CurMapSave);
            });
            //UITool.TryGetChildComponentByName<Button>("LoadBtn").onClick.AddListener(() =>
            //{
            //    PanelManager.Instance.AddPanel(new SavesPanel());
            //});
            UITool.TryGetChildComponentByName<Button>("CloseBtn").onClick.AddListener(() =>
            {
                PanelManager.RemoveTopPanel(this);
            });
            GameManager.Instance.PauseGame();
        }

        public override void OnExit()
        {
            base.OnExit();
            GameManager.Instance.RecoverGame();
        }
    }
}
