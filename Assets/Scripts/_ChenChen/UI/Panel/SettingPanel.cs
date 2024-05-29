using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class SettingPanel : PanelBase
    {
        static readonly string path = "UI/Panel/Menus/SettingPanel";
        public SettingPanel() : base(new UIType(path)) { }

        private string saveName = string.Empty;

        public override void OnEnter()
        {
            saveName = PlayManager.Instance.CurSave.SaveName;
            UITool.TryGetChildComponentByName<InputField>("SaveName").text = saveName;
            UITool.TryGetChildComponentByName<InputField>("SaveName").onValueChanged.AddListener((string content) =>
            {
                saveName = content;
            });
            UITool.TryGetChildComponentByName<Button>("SaveBtn").onClick.AddListener(() =>
            {
                PlayManager.Instance.Save(saveName);
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
