using ChenChen_BuildingSystem;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UISystem
{
    public class DetailViewPanel: PanelBase
    {
        static readonly string path = "UI/Panel/DetailViewPanel";
        private ThingBase thing;

        public Text ItemName { get; private set; }
        public Text Durability {  get; private set; }
        public Text Workload { get; private set; }
        public Text UserName { get; private set; }
        public Button DemolishBtn {  get; private set; }


        public DetailViewPanel(ThingBase thing, Callback onEnter, Callback onExit) : base(new UIType(path), onEnter, onExit) 
        {
            this.thing = thing;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            // �رղ˵��İ�ť
            UITool.TryGetChildComponentByName<Button>("Btn�ر�").onClick.AddListener(() =>
            {
                PanelManager.RemovePanel(this);
            });
            // ������ܵİ�ť
            DemolishBtn = UITool.TryGetChildComponentByName<Button>("Btn���");
            DemolishBtn.onClick.AddListener(() =>
            {
                thing.OnMarkDemolish();
            });
            DemolishBtn.gameObject.SetActive(thing.IsDismantlable);
            ItemName = UITool.TryGetChildComponentByName<Text>("ItemName");
            Durability = UITool.TryGetChildComponentByName<Text>("Durability");
            Workload = UITool.TryGetChildComponentByName<Text>("Workload");
            UserName = UITool.TryGetChildComponentByName<Text>("UserName");

        }      

        public void SetView(string itemName,int maxDur,int curDur,int workload,string userName = null)
        {
            ItemName.text = itemName;
            Durability.text = $"{maxDur} / {curDur}";
            Workload.text = workload.ToString();
            UserName.text = userName;
        }
    }
}