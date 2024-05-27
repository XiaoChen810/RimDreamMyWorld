using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class ButtonPanel : MonoBehaviour
    {
        public Button U;
        public Button T;
        public Button P;
        public Button B;

        private void Start()
        {
            if (U != null) U.onClick.AddListener(() =>
            {
                PanelManager.Instance.TogglePanel(new BuildingMenuPanel(), ChenChen_Scene.SceneType.Main, true);
            });
            if (T != null) T.onClick.AddListener(() =>
            {
                PanelManager.Instance.TogglePanel(new CropWorkSpacePanel(), ChenChen_Scene.SceneType.Main, true);
            });
            if (P != null) P.onClick.AddListener(() =>
            {
                PanelManager.Instance.TogglePanel(new PawnListPanel(), ChenChen_Scene.SceneType.Main, true);
            });
            if (B != null) B.onClick.AddListener(() =>
            {
                PanelManager.Instance.TogglePanel(new StoragesPanel(), ChenChen_Scene.SceneType.Main, true);
            });
        }
    }
}