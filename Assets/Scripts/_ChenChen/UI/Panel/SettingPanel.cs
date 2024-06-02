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
            // SAVE
            saveName = PlayManager.Instance.CurSaveName;
            UITool.TryGetChildComponentByName<InputField>("SaveName").text = saveName;
            UITool.TryGetChildComponentByName<InputField>("SaveName").onValueChanged.AddListener((string content) =>
            {
                Debug.Log("改变存档名字");
                saveName = content;
            });
            UITool.TryGetChildComponentByName<Button>("SaveBtn").onClick.AddListener(() =>
            {
                PlayManager.Instance.Save();
            });
            // BGM
            Slider bgmSlider = UITool.TryGetChildComponentByName<Slider>("BGM_Slider");
            bgmSlider.value = AudioManager.Instance.bgmSource.volume;
            bgmSlider.onValueChanged.AddListener((float value) =>
            {
                AudioManager.Instance.SetBGMVolume(value);
            });
            // SFX
            Slider sfxSlider = UITool.TryGetChildComponentByName<Slider>("SFX_Slider");
            sfxSlider.value = AudioManager.Instance.sfxSource.volume;
            sfxSlider.onValueChanged.AddListener((float value) =>
            {
                AudioManager.Instance.SetSFXVolume(value);
            });
            // Close
            UITool.TryGetChildComponentByName<Button>("CloseBtn").onClick.AddListener(() =>
            {
                PanelManager.RemoveTopPanel(this);
            });
            UITool.TryGetChildComponentByName<Button>("SaveAndQuitBtn").onClick.AddListener(() =>
            {
                Application.Quit();
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
