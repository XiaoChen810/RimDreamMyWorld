using ChenChen_Thing;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace ChenChen_UI
{
    public class Panel_Things : PanelBase
    {
        static readonly string path = "UI/Panel/Menus/ThingsPanel";

        private BuildingType ThingType;
        private GameObject content;
        private ThingPool thingPool;
        private Text descriptionText;

        public Panel_Things(BuildingType thingType) : base(new UIType(path))
        {
            this.ThingType = thingType;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            thingPool = ThingPool.Instance;
            content = UITool.GetChildByName("Content");
            descriptionText = UITool.TryGetChildComponentByName<Text>("DescriptionText");

            InitContent(ThingType);
            UITool.TryGetChildComponentByName<Button>("Btn关闭").onClick.AddListener(() =>
            {
                PanelManager.RemovePanel(this);
            });
        }

        /// <summary>
        /// 初始化内容
        /// </summary>
        private void InitContent(BuildingType type)
        {
            Dictionary<string, BuildingDef> dict = ThingSystemManager.Instance.ThingDefDictionary;

            // 根据蓝图字典,设置成对应的按钮添加到内容(content)中
            foreach (var item in dict)
            {
                if (item.Value.Type == type || (type == BuildingType.Architectural && item.Value.Type == BuildingType.Floor))
                {
                    thingPool.Init(content);
                    GameObject btnInstance = thingPool.pool.Get();
                    btnInstance.name = $"BtnBlueprint{item.Value.DefName}";
                    btnInstance.transform.Find("Image").GetComponent<Image>().sprite = item.Value.PreviewSprite;
                }
            }

            // 获取内容中的全部子物体
            CC_Button[] btnContents = UITool.GetChildByName("Content").GetComponentsInChildren<CC_Button>(true);
            foreach (var btn in btnContents)
            {
                btn.onClick.AddListener(() =>
                {
                    string name = btn.name.Replace("BtnBlueprint", "");
                    UseBlueprintByName(name);
                });

                btn.onMosueEnter = () =>
                {
                    string name = btn.name.Replace("BtnBlueprint", "");
                    var def = ThingSystemManager.Instance.GetThingDef(name);
                    var str = def.DefName + "\n" + def.Description;
                    descriptionText.text = StaticFuction.GetNstring(str, 20);
                };
            }


        }

        public override void OnExit()
        {
            foreach (Transform child in content.transform)
            {
                thingPool.pool.Release(child.gameObject);
            }
            base.OnExit();
        }
    }

    public class ThingPool : Singleton<ThingPool>
    {
        static readonly string btnPrefabPath = "UI/Component/BtnBlueprintDefault";

        public ObjectPool<GameObject> pool;
        private GameObject btnPrefab;
        private GameObject content;

        public ThingPool()
        {
            pool = new ObjectPool<GameObject>(P_Create, P_Get, P_Release, P_Destroy);
            btnPrefab = Resources.Load(btnPrefabPath) as GameObject;

            if (btnPrefab == null)
            {
                Debug.LogError("按钮的预制件为空, 检查位置: UI/Component/BtnBlueprintDefault");
            }
        }

        public void Init(GameObject content)
        {
            this.content = content;
        }

        #region - Pool -
        private GameObject P_Create()
        {
            GameObject @object = Object.Instantiate(btnPrefab);
            return @object;
        }
        private void P_Get(GameObject @object)
        {
            @object.SetActive(true);
            @object.transform.SetParent(content.transform, false);
            @object.transform.SetAsLastSibling();
        }
        private void P_Release(GameObject @object)
        {
            @object.transform.SetParent(null, false);
            @object.SetActive(false);
        }
        private void P_Destroy(GameObject @object)
        {
            GameObject.Destroy(@object);
        }
        #endregion
    }
}