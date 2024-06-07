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
            BaseSetting();
            CameraSetting();
            UITool.TryGetChildComponentByName<Button>("BtnBase").onClick.AddListener(() =>
            {
                UITool.GetChildByName("基础设置面板").SetActive(true);
                UITool.GetChildByName("镜头设置面板").SetActive(false);
            });       
            UITool.TryGetChildComponentByName<Button>("BtnCamera").onClick.AddListener(() =>
            {
                UITool.GetChildByName("镜头设置面板").SetActive(true);
                UITool.GetChildByName("基础设置面板").SetActive(false);
            });
            // Close
            UITool.TryGetChildComponentByName<Button>("CloseBtn").onClick.AddListener(() =>
            {
                PanelManager.RemoveTopPanel(this);
            });

            GameManager.Instance.PauseGame();
        }

        private void BaseSetting()
        {
            // SAVE
            saveName = PlayManager.Instance.CurSaveName;
            UITool.TryGetChildComponentByName<InputField>("SaveName").text = saveName == string.Empty ? "无存档" : "旧存档";
            UITool.TryGetChildComponentByName<Button>("SaveBtn").onClick.AddListener(() =>
            {
                PlayManager.Instance.Save();
                UITool.TryGetChildComponentByName<InputField>("SaveName").text = "已改变存档";
            });
            // Quit
            UITool.TryGetChildComponentByName<Button>("SaveAndQuitBtn").onClick.AddListener(() =>
            {
                Application.Quit();
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
        }

        private void CameraSetting()
        {
            CameraController cc = Camera.main.GetComponent<CameraController>();
            if (cc == null) return;
            // CameraSpeed
            Slider cameraSpeed_Slider = UITool.TryGetChildComponentByName<Slider>("CameraSpeed_Slider");
            cameraSpeed_Slider.value = (cc.MoveSpeed - cc.speedMin) / (cc.speedMax - cc.speedMin);
            cameraSpeed_Slider.onValueChanged.AddListener((float value) =>
            {
                cc.MoveSpeed = value * (cc.speedMax - cc.speedMin) + cc.speedMin;
            });
            // CameraSpeed
            Slider cameraZoom_Slider = UITool.TryGetChildComponentByName<Slider>("CameraZoom_Slider");
            cameraZoom_Slider.value = (cc.ZoomSpeed - cc.zoomSpeedMin) / (cc.zoomSpeedMax - cc.zoomSpeedMin);
            cameraZoom_Slider.onValueChanged.AddListener((float value) =>
            {
                cc.ZoomSpeed = value * (cc.zoomSpeedMax - cc.zoomSpeedMin) + cc.zoomSpeedMin;
            });
            Button b1 = UITool.TryGetChildComponentByName<Button>("UseKeyboardBtn_Open");
            Button b2 = UITool.TryGetChildComponentByName<Button>("UseKeyboardBtn_Close");
            Button b3 = UITool.TryGetChildComponentByName<Button>("UseEdgeBtn_Open");
            Button b4 = UITool.TryGetChildComponentByName<Button>("UseEdgeBtn_Close");
            b1.onClick.AddListener(() =>
            {
                b2.gameObject.SetActive(true);
                b1.gameObject.SetActive(false);
                cc.UseKeyboard = false;
            });
            b2.onClick.AddListener(() =>
            {
                b1.gameObject.SetActive(true);
                b2.gameObject.SetActive(false);
                cc.UseKeyboard = true;
            });
            b3.onClick.AddListener(() =>
            {
                b4.gameObject.SetActive(true);
                b3.gameObject.SetActive(false);
                cc.UseEdge = false;
            });
            b4.onClick.AddListener(() =>
            {
                b3.gameObject.SetActive(true);
                b4.gameObject.SetActive(false);
                cc.UseEdge = true;
            });
            b1.gameObject.SetActive(cc.UseKeyboard);
            b2.gameObject.SetActive(!cc.UseKeyboard);
            b3.gameObject.SetActive(cc.UseEdge);
            b4.gameObject.SetActive(!cc.UseEdge);
        }

        public override void OnExit()
        {
            base.OnExit();
            GameManager.Instance.RecoverGame();
        }
    }
}
