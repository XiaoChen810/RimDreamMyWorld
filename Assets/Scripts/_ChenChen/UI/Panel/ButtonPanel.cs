using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class ButtonPanel : MonoBehaviour
    {
        public Button U;
        public Button T;
        public Button P;
        public Button A;
        public Button B;
        public Button Setting;

        private void Start()
        {
            if (U != null) U.onClick.AddListener(() =>
            {
                PanelManager.Instance.TogglePanel(new BuildingMenuPanel(), true);
            });
            if (T != null) T.onClick.AddListener(() =>
            {
                PanelManager.Instance.TogglePanel(new CropWorkSpacePanel(), true);
            });
            if (P != null) P.onClick.AddListener(() =>
            {
                PanelManager.Instance.TogglePanel(new PawnListPanel(), true);
            });
            if (A != null) A.onClick.AddListener(() =>
            {
                PanelManager.Instance.TogglePanel(new AnimalsListPanel(), true);
            });
            if (B != null) B.onClick.AddListener(() =>
            {
                PanelManager.Instance.TogglePanel(new StoragesPanel(), true);
            });
            if (Setting != null) Setting.onClick.AddListener(() =>
            {
                PanelManager.Instance.TogglePanel(new SettingPanel(), true);
            });
        }
    }
}