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

        public Panel_Things(BuildingType thingType) : base(new UIType(path))
        {
            this.ThingType = thingType;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            thingPool = ThingPool.Instance;
            content = UITool.GetChildByName("Content");

            InitContent(ThingType);
            UITool.TryGetChildComponentByName<Button>("Btn�ر�").onClick.AddListener(() =>
            {
                PanelManager.RemovePanel(this);
            });
        }

        /// <summary>
        /// ��ʼ������
        /// </summary>
        private void InitContent(BuildingType type)
        {
            Dictionary<string, BuildingDef> dict = ThingSystemManager.Instance.ThingDefDictionary;

            // ������ͼ�ֵ�,���óɶ�Ӧ�İ�ť��ӵ�����(content)��
            foreach (var item in dict)
            {
                if (item.Value.Type == type)
                {
                    thingPool.Init(content);
                    GameObject btnInstance = thingPool.pool.Get();
                    btnInstance.name = $"BtnBlueprint{item.Value.DefName}";
                    btnInstance.transform.Find("Image").GetComponent<Image>().sprite = item.Value.PreviewSprite;
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
                Debug.LogError("��ť��Ԥ�Ƽ�Ϊ��, ���λ��: UI/Component/BtnBlueprintDefault");
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