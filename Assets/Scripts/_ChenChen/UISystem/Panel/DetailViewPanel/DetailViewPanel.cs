using ChenChen_BuildingSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UISystem
{
    public abstract class DetailViewPanel : PanelBase
    {
        static readonly string path = "UI/Panel/Menus/DetailViewPanel";

        public Text ItemName { get; private set; }
        public List<Text> Texts { get; private set; }
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
            Texts = UITool.GetChildByName("TextContent").GetComponentsInChildren<Text>().ToList();
        }

        public void SetView(string itemName, List<string> content)
        {
            ItemName.text = itemName;
            if(content.Count > Texts.Count)
            {
                Debug.LogWarning($"最大加载{Texts.Count}行内容");
            }
            for (int i = 0; i < Texts.Count; i++)
            {
                if(i < content.Count)
                {
                    Texts[i].text = content[i];
                }
                else
                {
                    Texts[i].text = string.Empty;
                }
            }
        }
    }
}
