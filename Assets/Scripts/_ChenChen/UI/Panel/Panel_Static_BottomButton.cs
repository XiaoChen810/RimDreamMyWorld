using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class Panel_Static_BottomButton : MonoBehaviour
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
                PanelManager.Instance.TogglePanel(new Panel_BuildingMenu(), true);
            });
            if (T != null) T.onClick.AddListener(() =>
            {
                PanelManager.Instance.TogglePanel(new Panel_WorkSpace_Crop(), true);
            });
            if (P != null) P.onClick.AddListener(() =>
            {
                PanelManager.Instance.TogglePanel(new Panel_PawnList(), true);
            });
            if (A != null) A.onClick.AddListener(() =>
            {
                PanelManager.Instance.TogglePanel(new Panel_AnimalsList(), true);
            });
            if (B != null) B.onClick.AddListener(() =>
            {
                PanelManager.Instance.TogglePanel(new Panel_Storages(), true);
            });
            if (Setting != null) Setting.onClick.AddListener(() =>
            {
                PanelManager.Instance.TogglePanel(new Panel_Setting(), true);
            });
        }
    }
}