using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChenChen_BuildingSystem;

namespace ChenChen_UISystem
{
    /// <summary>
    /// UI面板的基类
    /// </summary>
    public abstract class PanelBase
    {
        public UIType UIType { get; private set; }

        public UITool UITool { get; private set; }

        public PanelManager PanelManager { get; private set; }

        public UIManager UIManager { get; private set; }

        // 定义回调函数的委托类型
        public delegate void Callback();

        // 声明面板创建和退出的回调函数
        private Callback onEnterCallback;
        private Callback onExitCallback;

        /// <summary>
        ///  带回调函数的构造函数
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
        /// 使用UIType构造函数，但一般接受静态路径作为参数<br></br>
        /// 示例用法：
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
        ///  初始化UITool
        /// </summary>
        /// <param name="uITool"></param>
        public void Init(UITool uITool)
        {
            UITool = uITool;
        }

        /// <summary>
        ///  初始化PanelManager
        /// </summary>
        /// <param name="panelManager"></param>
        public void Init(PanelManager panelManager)
        {
            PanelManager = panelManager;
        }

        /// <summary>
        ///  初始化UIManager
        /// </summary>
        /// <param name="uiManager"></param>
        public void Init(UIManager uiManager)
        {
            UIManager = uiManager;
        }

        /// <summary>
        /// 创建时的调用，默认调用进入回调
        /// </summary>
        public virtual void OnEnter()
        {
            onEnterCallback?.Invoke();
        }

        /// <summary>
        /// 默认将GameObject SetActive to false
        /// </summary>
        public virtual void OnPause()
        {
            UITool.GetOrAddComponent<Transform>().gameObject.SetActive(false);
        }

        /// <summary>
        /// 默认将GameObject SetActive to true
        /// </summary>
        public virtual void OnResume()
        {
            UITool.GetOrAddComponent<Transform>().gameObject.SetActive(true);
        }

        /// <summary>
        /// 删除时的调用，默认删除GameObject 和 调用退出回调函数
        /// </summary>
        public virtual void OnExit()
        {
            onExitCallback?.Invoke();
            GameObject.Destroy(UITool.GetOrAddComponent<Transform>().gameObject);
        }

        /// <summary>
        /// 调用BuildingSystemManager，通过 name 选择按钮对应蓝图
        /// 然后关闭面板
        /// </summary>
        /// <param name="name"></param>
        protected void UseBlueprintByName(string name)
        {
            BuildingSystemManager.Instance.UseBlueprint(name);
            PanelManager.RemovePanel(this);
        }

        /// <summary>
        /// 初始化内容
        /// </summary>
        protected void InitContent(Dictionary<string, BlueprintData> dict) 
        {
            // 获取装内容的子物体
            GameObject content = UITool.GetChildByName("Content");
            // 检查是否有GridLayoutGroup组件
            GridLayoutGroup glg = UITool.TryGetChildComponentByName<GridLayoutGroup>("Content");
            // 获取按钮的预制件
            GameObject btnPrefab = Resources.Load("UI/Component/BtnBkueprintDefault/BtnBlueprintDefault") as GameObject;
            if (btnPrefab == null)
            {
                Debug.LogWarning("按钮的预制件为空");
                PanelManager.RemovePanel(this);
                return;
            }
            // 根据蓝图字典,设置成对应的按钮添加到内容中
            foreach (var item in dict)
            {
                GameObject btnInstance = Object.Instantiate(btnPrefab);
                btnInstance.name = $"BtnBlueprint{item.Value.Name}";
                GameObject btnImage = btnInstance.transform.Find("Image").gameObject;
                btnImage.GetComponent<Image>().sprite = item.Value.PreviewSprite;

                btnInstance.transform.SetParent(content.transform, false);
            }

            // 获取内容中的全部子物体
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