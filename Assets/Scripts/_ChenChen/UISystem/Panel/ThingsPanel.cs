using ChenChen_BuildingSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenChen_UISystem
{
    public class ThingsPanel : PanelBase
    {
        static readonly string path = "UI/Panel/Menus/ThingsPanel";
        private ThingType ThingType;
        public ThingsPanel(ThingType thingType) : base(new UIType(path))
        {
            this.ThingType = thingType;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            InitContent(ThingType);
            UITool.TryGetChildComponentByName<Button>("Btn�ر�").onClick.AddListener(() =>
            {
                PanelManager.RemoveTopPanel(this);
            });
        }

        /// <summary>
        /// ��ʼ������
        /// </summary>
        private void InitContent(ThingType type)
        {
            Dictionary<string, ThingDef> dict = ThingSystemManager.Instance.ThingDefDictionary;
            // ��ȡװ���ݵ�������
            GameObject content = UITool.GetChildByName("Content");
            // ����Ƿ���GridLayoutGroup���
            GridLayoutGroup glg = UITool.TryGetChildComponentByName<GridLayoutGroup>("Content");
            // ��ȡ��ť��Ԥ�Ƽ�
            GameObject btnPrefab = Resources.Load("UI/Component/BtnBlueprintDefault") as GameObject;
            if (btnPrefab == null)
            {
                Debug.LogError("��ť��Ԥ�Ƽ�Ϊ��, ���λ��: UI/Component/BtnBlueprintDefault");
                PanelManager.RemoveTopPanel(this);
                return;
            }
            // ������ͼ�ֵ�,���óɶ�Ӧ�İ�ť��ӵ�����(content)��
            foreach (var item in dict)
            {
                if (item.Value.Type == type)
                {
                    GameObject btnInstance = Object.Instantiate(btnPrefab);
                    btnInstance.name = $"BtnBlueprint{item.Value.DefName}";
                    GameObject btnImage = btnInstance.transform.Find("Image").gameObject;
                    btnImage.GetComponent<Image>().sprite = item.Value.PreviewSprite;
                    btnInstance.transform.SetParent(content.transform, false);
                }
            }

            // ��ȡ�����е�ȫ��������
            Button[] btnContents = UITool.GetChildByName("Content").GetComponentsInChildren<Button>(true);
            foreach (var btn in btnContents)
            {
                btn.onClick.AddListener(() =>
                {
                    string name = btn.name.Replace("BtnBlueprint", "");
                    UseBlueprintByName(name);
                });
            }
        }
    }
}