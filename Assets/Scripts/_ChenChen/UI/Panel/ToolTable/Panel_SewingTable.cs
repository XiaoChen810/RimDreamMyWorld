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

        private Thing_SewingTable sewingTable;

        public Panel_SewingTable(Thing_SewingTable sewingTable) : base(new UIType(path))
        {
            this.sewingTable = sewingTable;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            // 通过xml读取所有衣服类定义
            var xmlLoader = XmlLoader.Instance;
            List<ApparelDef> apparelDefs = xmlLoader.Get<ApparelDef>(XmlLoader.Def_Apparel);

            GameObject appealInfoPrefab = Resources.Load(path_appealInfo) as GameObject;
            Transform content = UITool.GetChildByName("Content").transform;

            foreach(var apparelDef in apparelDefs)
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

            UITool.TryGetChildComponentByName<Button>("CancelDoing").onClick.AddListener(CancelCurrentDoing);

            UITool.GetChildByName("CurrentDoing").SetActive(false);
            if (sewingTable.CurrentApparelDef != null)
            {
                UITool.TryGetChildComponentByPath<Image>("CurrentDoing/iconBG/Icon").sprite = sewingTable.CurrentApparelDef.sprite;
                UITool.GetChildByName("CurrentDoing").SetActive(true);
            }

            UITool.TryGetChildComponentByName<Button>("Btn关闭").onClick.AddListener(() =>
            {
                PanelManager.RemovePanel(this);
            });
        }

        public override void OnExit()
        {
            sewingTable.isOpenMenu = false;
            base.OnExit();
        }

        private void SetCurrnetDoing(ApparelDef def)
        {
            sewingTable.CurrentApparelDef = def;
            UITool.TryGetChildComponentByPath<Image>("CurrentDoing/iconBG/Icon").sprite = def.sprite;
            UITool.GetChildByName("CurrentDoing").SetActive(true);
        }

        private void CancelCurrentDoing()
        {
            sewingTable.CurrentApparelDef = null;
            UITool.GetChildByName("CurrentDoing").SetActive(false);
        }
    }
}