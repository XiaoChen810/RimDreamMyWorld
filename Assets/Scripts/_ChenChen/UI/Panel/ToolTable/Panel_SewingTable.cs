using ChenChen_Thing;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ChenChen_Core;

namespace ChenChen_UI
{
    public class Panel_SewingTable : PanelBase
    {
        private static readonly string path = "UI/Panel/Menus/SewingTablePanel";
        private static readonly string path_appealInfo = "UI/Component/Info/ApparelInfo";

        private readonly Thing_SewingTable sewingTable;

        public Panel_SewingTable(Thing_SewingTable sewingTable) : base(new UIType(path))
        {
            this.sewingTable = sewingTable;
        }

        public override void OnEnter()
        {
            InitApparelList();

            UITool.TryGetChildComponentByName<Button>("CancelDoing").onClick.AddListener(CancelCurrentDoing);

            UITool.TryGetChildComponentByName<Button>("PlusDemand").onClick.AddListener(() =>
            {
                sewingTable.NumberOfWaitingToMade--;
                UITool.TryGetChildComponentByName<Text>("NumberOfWaitingToMade").text = sewingTable.NumberOfWaitingToMade.ToString();
            });

            UITool.TryGetChildComponentByName<Button>("ReduceDemand").onClick.AddListener(() =>
            {
                sewingTable.NumberOfWaitingToMade++;
                UITool.TryGetChildComponentByName<Text>("NumberOfWaitingToMade").text = sewingTable.NumberOfWaitingToMade.ToString();
            });

            UITool.GetChildByName("CurrentDoing").SetActive(false);

            if (sewingTable.CurrentApparelDef != null)
            {
                UITool.TryGetChildComponentByPath<Image>("CurrentDoing/iconBG/Icon").sprite = sewingTable.CurrentApparelDef.sprite;
                UITool.TryGetChildComponentByName<Text>("NumberOfWaitingToMade").text = sewingTable.NumberOfWaitingToMade.ToString();
                UITool.GetChildByName("CurrentDoing").SetActive(true);
            }

            UITool.TryGetChildComponentByName<Button>("Btn关闭").onClick.AddListener(() =>
            {
                PanelManager.RemovePanel(this);
            });
        }

        public override void OnExit()
        {
            sewingTable.IsOpenMenu = false;
            base.OnExit();
        }

        private void InitApparelList()
        {
            var xmlLoader = XmlLoader.Instance;
            List<ApparelDef> apparelDefs = xmlLoader.Get<ApparelDef>(XmlLoader.Def_Apparel);
            Transform content = UITool.GetChildByName("Content").transform;
            GameObject appealInfoPrefab = Resources.Load(path_appealInfo) as GameObject;

            foreach (var apparelDef in apparelDefs)
            {
                var obj = GameObject.Instantiate(appealInfoPrefab);
                obj.transform.SetParent(content);

                var appealInfo = obj.GetComponent<ApparelInfo>();
                appealInfo.Set(apparelDef);

                appealInfo.onClick.AddListener(() =>
                {
                    SetCurrnetDoing(apparelDef);
                });
            }
        }

        // 设置当前任务
        private void SetCurrnetDoing(ApparelDef def)
        {
            sewingTable.CurrentApparelDef = def;
            sewingTable.NumberOfWaitingToMade = 1;
            UITool.TryGetChildComponentByPath<Image>("CurrentDoing/iconBG/Icon").sprite = def.sprite;
            UITool.TryGetChildComponentByName<Text>("NumberOfWaitingToMade").text = sewingTable.NumberOfWaitingToMade.ToString();
            UITool.GetChildByName("CurrentDoing").SetActive(true);
        }

        // 取消当前任务
        private void CancelCurrentDoing()
        {
            sewingTable.CurrentApparelDef = null;
            sewingTable.NumberOfWaitingToMade = 0;
            UITool.GetChildByName("CurrentDoing").SetActive(false);
        }
    }
}