using UnityEngine;
using ChenChen_MapGenerator;
using ChenChen_AI;

namespace ChenChen_BuildingSystem
{
    public abstract class ThingBase : PermissionBase, IBlueprint, IDemolish, IDetailView 
    {
        /// <summary>
        /// 物品自身的定义
        /// </summary>
        public ThingDef Def;

        /// <summary>
        /// 物品所在的地图名
        /// </summary>
        public string MapName { get; protected set; }

        /// <summary>
        /// 物品是可以拆除的
        /// </summary>
        public bool CanDemolish { get; protected set; }

        /// <summary>
        /// 物品自身碰撞体
        /// </summary>
        public BoxCollider2D ColliderSelf { get; protected set; }

        // 物品当前耐久度
        public int CurDurability { get; protected set; }

        // 物品建造所需工作量
        public int WorkloadBuilt
        {
            get { return Def.Workload; }
        }

        // 物品拆除所需工作量
        public int WorkloadDemolished
        {
            get { return Mathf.CeilToInt(Def.Workload * 0.5f); }
        }

        // 物品最大耐久度
        public int MaxDurability
        {
            get { return Def.Durability; }
        }


        // 物品工作量
        protected int _workload;
        public int Workload
        {
            get
            {
                return _workload;
            }
            set
            {
                _workload = value > 0 ? value : 0;
            }
        }

        // 生命周期
        private BuildingLifeStateType _lifeState = 0;
        public BuildingLifeStateType LifeState
        {
            get
            {
                return _lifeState;
            }
            set
            {
                if (value != _lifeState)
                {
                    switch (value)
                    {
                        case BuildingLifeStateType.MarkBuilding:
                            OnMarkBuild();
                            break;
                        case BuildingLifeStateType.FinishedBuilding:
                            OnComplete();
                            break;
                        case BuildingLifeStateType.MarkDemolished:
                            OnMarkDemolish();
                            break;
                        case BuildingLifeStateType.FinishedDemolished:
                            OnDemolished();
                            break;

                    }
                    _lifeState = value;
                }
                else
                {
                    if (value != BuildingLifeStateType.None)
                        Debug.LogWarning($"物品{this.name}(position: {this.transform.position})的生命周期已经处于 {value}，无需切换");
                }
            }
        }

        // 细节视图
        protected DetailView _detailView;
        public DetailView DetailView
        {
            get
            {
                if (_detailView == null)
                {
                    if (!TryGetComponent<DetailView>(out _detailView))
                    {
                        _detailView = gameObject.AddComponent<DetailView_Thing>();
                    }
                }
                return _detailView;
            }
        }

        // 绘制GUI
        public bool DrawOutline_Sprite;    // 根据Sprite Render画
        public bool DrawOutline_Collider;    // 根据Box Collider2D画
        private Color centerColor = new Color(0f, 0f, 1f, 0.5f); // 透明蓝色
        private Color outlineColor = Color.white; // 纯白色
        private float outlineWidth = 2f;

        private Texture2D centerTexture;
        private Texture2D outlineTexture;
        protected virtual void OnEnable()
        {
            gameObject.name = gameObject.name.Replace("(Clone)", "");
            CurDurability = MaxDurability;
            ColliderSelf = GetComponent<BoxCollider2D>();
            tag = "Thing";

            // 创建中心透明蓝色纹理
            centerTexture = new Texture2D(1, 1);
            centerTexture.SetPixel(0, 0, centerColor);
            centerTexture.Apply();

            // 创建纯白色纹理用于绘制边框
            outlineTexture = new Texture2D(1, 1);
            outlineTexture.SetPixel(0, 0, outlineColor);
            outlineTexture.Apply();
        }
        private void OnGUI()
        {
            if (DrawOutline_Collider)
            {
                // 计算建造物体的边界框
                Bounds bounds = GetComponent<Collider2D>().bounds;

                // 计算建造物体边界框在屏幕上的位置和大小
                Vector3 screenBoundsMin = Camera.main.WorldToScreenPoint(bounds.min);
                Vector3 screenBoundsMax = Camera.main.WorldToScreenPoint(bounds.max);
                Vector3 screenSize = screenBoundsMax - screenBoundsMin;

                // 绘制中心透明蓝色矩形
                GUI.DrawTexture(new Rect(screenBoundsMin.x, Screen.height - screenBoundsMax.y, screenSize.x, screenSize.y), centerTexture);

                // 绘制边框
                GUI.DrawTexture(new Rect(screenBoundsMin.x, Screen.height - screenBoundsMax.y, screenSize.x, outlineWidth), outlineTexture); // 上边框
                GUI.DrawTexture(new Rect(screenBoundsMin.x, Screen.height - screenBoundsMax.y, outlineWidth, screenSize.y), outlineTexture); // 左边框
                GUI.DrawTexture(new Rect(screenBoundsMax.x - outlineWidth, Screen.height - screenBoundsMax.y, outlineWidth, screenSize.y), outlineTexture); // 右边框
                GUI.DrawTexture(new Rect(screenBoundsMin.x, Screen.height - screenBoundsMin.y, screenSize.x, outlineWidth), outlineTexture); // 下边框
            }
            if (DrawOutline_Sprite)
            {
                // 计算建造物体的边界框
                Bounds bounds = GetComponent<SpriteRenderer>().bounds;

                // 计算建造物体边界框在屏幕上的位置和大小
                Vector3 screenBoundsMin = Camera.main.WorldToScreenPoint(bounds.min);
                Vector3 screenBoundsMax = Camera.main.WorldToScreenPoint(bounds.max);
                Vector3 screenSize = screenBoundsMax - screenBoundsMin;

                // 绘制边框
                GUI.DrawTexture(new Rect(screenBoundsMin.x, Screen.height - screenBoundsMax.y, screenSize.x, outlineWidth), outlineTexture); // 上边框
                GUI.DrawTexture(new Rect(screenBoundsMin.x, Screen.height - screenBoundsMax.y, outlineWidth, screenSize.y), outlineTexture); // 左边框
                GUI.DrawTexture(new Rect(screenBoundsMax.x - outlineWidth, Screen.height - screenBoundsMax.y, outlineWidth, screenSize.y), outlineTexture); // 右边框
                GUI.DrawTexture(new Rect(screenBoundsMin.x, Screen.height - screenBoundsMin.y, screenSize.x, outlineWidth), outlineTexture); // 下边框
            }
        }

        /// <summary>
        /// 检查是否能够在指定位置放置指定的游戏对象 
        /// </summary>
        /// <returns></returns>
        public virtual bool CanBuildHere()
        {
            // 获取待放置对象的 Collider2D 组件
            Collider2D collider = ColliderSelf;
            // 获取待放置对象 Collider2D 的边界框信息
            Bounds bounds = collider.bounds;
            // 计算碰撞体在世界空间中的中心位置
            Vector2 center = bounds.center;
            // 执行碰撞检测，只检测指定图层的碰撞器
            Collider2D[] colliders = Physics2D.OverlapBoxAll(center, bounds.size, 0f);

            // 遍历检测到的碰撞器，如果有任何一个碰撞器存在，则返回 false，表示无法放置游戏对象
            foreach (Collider2D otherCollider in colliders)
            {
                if (otherCollider.gameObject != this.gameObject) // 忽略自己游戏对象的碰撞
                {
                    return false;
                }
            }

            // 如果没有任何碰撞器存在，则表示可以放置游戏对象
            return true;
        }


        // 实现接口中定义的属性和方法
        public abstract void OnPlaced(BuildingLifeStateType initial_State, string mapName);
        public virtual void OnMarkBuild() { throw new System.NotImplementedException(); }
        public virtual void OnBuild(int value) { throw new System.NotImplementedException(); }
        public virtual void OnComplete() { throw new System.NotImplementedException(); }
        public virtual void OnCancel() { throw new System.NotImplementedException(); }
        public virtual void OnInterpret() { throw new System.NotImplementedException(); }
        public virtual void OnMarkDemolish() { throw new System.NotImplementedException(); }
        public virtual void OnDemolish(int value) { throw new System.NotImplementedException(); }
        public virtual void OnDemolished() { throw new System.NotImplementedException(); }
    }
}