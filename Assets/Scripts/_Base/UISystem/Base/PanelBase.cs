using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChenChen_BuildingSystem;

namespace ChenChen_UISystem
{
    /// <summary>
    /// UI���Ļ���
    /// </summary>
    public abstract class PanelBase
    {
        public UIType UIType { get; private set; }

        public UITool UITool { get; private set; }

        /// <summary>
        /// �Լ����ڵ�PanelManager
        /// </summary>
        public PanelManager PanelManager { get; private set; }

        public UIManager UIManager { get; private set; }

        // ����ص�������ί������
        public delegate void Callback();

        // ������崴�����˳��Ļص�����
        private Callback onEnterCallback;
        private Callback onExitCallback;

        public bool IsStopping { get; private set; }

        /// <summary>
        ///  ���ص������Ĺ��캯��
        /// </summary>
        /// <param name="UIType"></param>
        /// <param name="onEnter"></param>
        /// <param name="onExit"></param>
        protected PanelBase(UIType UIType,Callback onEnter,Callback onExit)
        {
            this.UIType = UIType;
            this.onEnterCallback = onEnter;
            this.onExitCallback = onExit;
        }

        /// <summary>
        /// ʹ��UIType���캯������һ����ܾ�̬·����Ϊ����<br></br>
        /// ʾ���÷���
        /// <code>
        /// static readonly string path = "UI/Panel/DynamicPath";
        /// public Panel() : base(new UIType(path)) { }
        /// </code>
        /// </summary>
        protected PanelBase(UIType uIType)
        {
            UIType = uIType;
        }

        /// <summary>
        ///  ��ʼ��UITool
        /// </summary>
        /// <param name="uITool"></param>
        public void Init(UITool uITool)
        {
            UITool = uITool;
        }

        /// <summary>
        ///  ��ʼ��PanelManager
        /// </summary>
        /// <param name="panelManager"></param>
        public void Init(PanelManager panelManager)
        {
            PanelManager = panelManager;
        }

        /// <summary>
        ///  ��ʼ��UIManager
        /// </summary>
        /// <param name="uiManager"></param>
        public void Init(UIManager uiManager)
        {
            UIManager = uiManager;
        }

        /// <summary>
        /// ����ʱ�ĵ��ã�Ĭ�ϵ��ý���ص�
        /// </summary>
        public virtual void OnEnter()
        {
            onEnterCallback?.Invoke();
        }

        /// <summary>
        /// Ĭ�Ͻ�GameObject SetActive to false
        /// </summary>
        public virtual void OnPause()
        {
            if (UITool.TryGetOrAddComponent<Transform>(out Transform trans))
            {
                trans.gameObject.SetActive(false);
            }
            IsStopping = true;
        }

        /// <summary>
        /// Ĭ�Ͻ�GameObject SetActive to true
        /// </summary>
        public virtual void OnResume()
        {
            if (UITool.TryGetOrAddComponent<Transform>(out Transform trans))
            {
                trans.gameObject.SetActive(true);
            }
            IsStopping = false;
        }

        /// <summary>
        /// ɾ��ʱ�ĵ��ã�Ĭ��ɾ��GameObject �� �����˳��ص�����
        /// </summary>
        public virtual void OnExit()
        {
            onExitCallback?.Invoke();
            if (UITool.TryGetOrAddComponent<Transform>(out Transform trans))
            {
                GameObject.Destroy(trans.gameObject);
            }
        }

        /// <summary>
        /// ����BuildingSystemManager��ͨ�� name ѡ��ť��Ӧ��ͼ
        /// Ȼ��ر����
        /// </summary>
        /// <param name="name"></param>
        protected void UseBlueprintByName(string name)
        {
            BuildingSystemManager.Instance.OpenBuildingMode(name);
            PanelManager.RemovePanel(this);
        }

        /// <summary>
        /// ��ʼ������
        /// </summary>
        protected void InitContent(ThingType type) 
        {
            Dictionary<string, ThingDef> dict = BuildingSystemManager.Instance.ThingDefDictionary;
            // ��ȡװ���ݵ�������
            GameObject content = UITool.GetChildByName("Content");
            // ����Ƿ���GridLayoutGroup���
            GridLayoutGroup glg = UITool.TryGetChildComponentByName<GridLayoutGroup>("Content");
            // ��ȡ��ť��Ԥ�Ƽ�
            GameObject btnPrefab = Resources.Load("UI/Component/BtnBlueprintDefault") as GameObject;
            if (btnPrefab == null)
            {
                Debug.LogError("��ť��Ԥ�Ƽ�Ϊ��, ���λ��: UI/Component/BtnBlueprintDefault");
                PanelManager.RemovePanel(this);
                return;
            }
            // ������ͼ�ֵ�,���óɶ�Ӧ�İ�ť��ӵ�������
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
            foreach(var btn  in btnContents)
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