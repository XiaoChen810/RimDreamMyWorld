using ChenChen_BuildingSystem;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UISystem
{
    public abstract class DetailViewPanel : PanelBase
    {
        static readonly string path = "UI/Panel/Menus/DetailViewPanel";

        public Text ItemName { get; private set; }
        public Text Durability { get; private set; }
        public Text Workload { get; private set; }
        public Text UserName { get; private set; }
        public Button DemolishBtn { get; private set; }


        public DetailViewPanel(Callback onEnter, Callback onExit) : base(new UIType(path), onEnter, onExit)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            // 关闭菜单的按钮
            UITool.TryGetChildComponentByName<Button>("Btn关闭").onClick.AddListener(() =>
            {
                PanelManager.RemoveTopPanel(this);
            });
            DemolishBtn = UITool.TryGetChildComponentByName<Button>("Btn拆除");
            DemolishBtn.gameObject.SetActive(false);

            ItemName = UITool.TryGetChildComponentByName<Text>("ItemName");
            Durability = UITool.TryGetChildComponentByName<Text>("Durability");
            Workload = UITool.TryGetChildComponentByName<Text>("Workload");
            UserName = UITool.TryGetChildComponentByName<Text>("UserName");

        }

        public void SetView(string itemName, int maxDur, int curDur, int workload, string userName = null)
        {
            ItemName.text = itemName;
            Durability.text = $"{maxDur} / {curDur}";
            Workload.text = workload.ToString();
            UserName.text = userName;
        }
    }
}
