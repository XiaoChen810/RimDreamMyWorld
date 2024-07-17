using ChenChen_Thing;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public abstract class DetailViewPanel : PanelBase
    {
        static readonly string path = "UI/Panel/Menus/DetailViewPanel";

        public Text ItemName { get; private set; }
        public List<Text> Texts { get; private set; }
        public List<Button> Buttons { get; private set; }

        public DetailViewPanel(Callback onEnter, Callback onExit) : base(new UIType(path), onEnter, onExit)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            //// 关闭菜单的按钮
            //UITool.TryGetChildComponentByName<Button>("Btn关闭").onClick.AddListener(() =>
            //{
            //    PanelManager.RemoveTopPanel(this);
            //});
            ItemName = UITool.TryGetChildComponentByName<Text>("ItemName");
            Texts = UITool.GetChildByName("TextContent").GetComponentsInChildren<Text>().ToList();
            Buttons = UITool.GetChildByName("BtnContent").GetComponentsInChildren<Button>().ToList();
            foreach (var btn in  Buttons)
            {
                btn.gameObject.SetActive(false);
            }
        }

        public void SetView(string titleName, List<string> content)
        {
            ItemName.text = titleName;
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

        private Dictionary<string, Button> _btnDict = new Dictionary<string, Button>();
        public void SetButton(string btnText, UnityAction onClick)
        {
            if (_btnDict.ContainsKey(btnText)) return;

            foreach(var btn in Buttons)
            {
                if (!btn.gameObject.activeSelf)
                {
                    btn.gameObject.SetActive(true);
                    btn.GetComponentInChildren<Text>().text = btnText;
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(onClick);
                    _btnDict.Add(btnText, btn);
                    return;
                }
            }
            Debug.LogWarning("使用的Button已达最大限度");
        }

        public void RemoveButton(string btnText)
        {
            if (_btnDict.ContainsKey(btnText))
            {
                _btnDict[btnText].gameObject.SetActive(false);
                _btnDict.Remove(btnText);
            }
        }

        public void RemoveAllButton()
        {
            foreach(var btn in _btnDict)
            {
                btn.Value.gameObject.SetActive(false);
            }
            _btnDict.Clear();
        }
    }
}
