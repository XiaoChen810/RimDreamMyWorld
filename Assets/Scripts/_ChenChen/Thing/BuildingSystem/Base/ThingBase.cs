using UnityEngine;
using ChenChen_Map;
using ChenChen_AI;
using static UnityEngine.Rendering.DebugUI;
using System.Linq;
using ChenChen_UI;
using UnityEngine.Tilemaps;

namespace ChenChen_Thing
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

        public SpriteRenderer SR {  get; protected set; }

        /// <summary>
        /// 物品当前耐久度
        /// </summary>
        public int CurDurability { get; protected set; }

        /// <summary>
        /// 物品建造所需工作量
        /// </summary>
        public int WorkloadBuilt
        {
            get { return Def.Workload; }
        }

        /// <summary>
        /// 物品拆除所需工作量
        /// </summary>
        public int WorkloadDemolished
        {
            get { return Mathf.CeilToInt(Def.Workload * 0.5f); }
        }

        /// <summary>
        /// 物品最大耐久度
        /// </summary>
        public int MaxDurability
        {
            get { return Def.Durability; }
        }


        protected int _workload;
        /// <summary>
        /// 物品剩余工作量
        /// </summary>
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

        [SerializeField] protected BuildingLifeStateType _lifeState = 0;
        /// <summary>
        /// 生命周期
        /// </summary>
        public BuildingLifeStateType LifeState
        {
            get
            {
                return _lifeState;
            }
        }

        /// <summary>
        /// 切换生命周期
        /// </summary>
        /// <param name="change"></param>
        public void ChangeLifeState(BuildingLifeStateType change)
        {
            if (change != _lifeState)
            {
                switch (change)
                {
                    case BuildingLifeStateType.MarkBuilding:
                        OnMarkBuild();
                        break;
                    case BuildingLifeStateType.FinishedBuilding:
                        OnCompleteBuild();
                        break;
                    case BuildingLifeStateType.MarkDemolished:
                        OnMarkDemolish();
                        break;
                    case BuildingLifeStateType.FinishedDemolished:
                        OnDemolished();
                        break;
                }
                _lifeState = change;
            }
            else
            {
                if (change != BuildingLifeStateType.None)
                    Debug.LogWarning($"物品{this.name}(position: {this.transform.position})的生命周期已经处于 {change}，无需切换");
            }
        }

        // 放置瓦片（如果有）的瓦片地图名
        [SerializeField] protected string _tilemapName = "Building";

        // 细节视图
        protected DetailView _detailView;
        public virtual DetailView DetailView
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
        public bool DrawOutline_Collider;    // 根据Box Collider2D画
        private Color centerColor = new Color(0f, 0f, 1f, 0f); // 透明蓝色
        private Color outlineColor = Color.white; // 纯白色
        private float outlineWidth = 2f;

        private Texture2D centerTexture;
        private Texture2D outlineTexture;
        protected virtual void OnEnable()
        {
            gameObject.name = gameObject.name.Replace("(Clone)", "");
            tag = "Thing";

            ColliderSelf = GetComponent<BoxCollider2D>();
            ColliderSelf.isTrigger = true;

            SR = GetComponent<SpriteRenderer>();
            SR.sortingLayerName = "Middle";
            SR.sortingOrder = -(int)transform.position.y;

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
            if (GameManager.Instance.CinematicMode) return;
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
        }

        private void OnDestroy()
        {
            if (!Application.isPlaying) return;
            if (Def.TileBase != null)
            {
                if (MapManager.Instance.TryGetTilemap(_tilemapName, true, out Tilemap buildingTilemap))
                {
                    buildingTilemap.SetTile(StaticFuction.VectorTransToInt(transform.position), null);
                }
            }
            if (_detailView != null && _detailView.IsPanelOpen)
            {
                PanelManager panel = DetailViewManager.Instance.PanelManager;
                panel.RemoveTopPanel(panel.GetTopPanel());
            }
            if (Def.IsObstacle)
            {
                if (AstarPath.active != null)
                {
                    Bounds bounds = ColliderSelf.bounds;
                    AstarPath.active.UpdateGraphs(bounds);
                }
            }
            ThingSystemManager.Instance.RemoveThing(this.gameObject);
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

            // 遍历检测到的碰撞器，如果有任何一个碰撞器存在,除了地板，则返回 false，表示无法放置游戏对象
            foreach(var coll in colliders)
            {
                if (coll.gameObject == this.gameObject) continue;
                if (coll.CompareTag("Pawn")) continue;
                if (coll.CompareTag("Floor")) continue;
                return false;              
            }

            // 如果没有任何碰撞器存在，则表示可以放置游戏对象
            return true;
        }


        // 实现接口中定义的属性和方法
        public abstract void OnPlaced(BuildingLifeStateType initial_State, string mapName);
        public virtual void OnMarkBuild() { throw new System.NotImplementedException(); }
        public virtual void OnBuild(int value) { throw new System.NotImplementedException(); }
        public virtual void OnCompleteBuild() { throw new System.NotImplementedException(); }
        public virtual void OnCancelBuild() { throw new System.NotImplementedException(); }
        public virtual void OnInterpretBuild() { throw new System.NotImplementedException(); }
        public virtual void OnMarkDemolish() { throw new System.NotImplementedException(); }
        public virtual void OnDemolish(int value) { throw new System.NotImplementedException(); }
        public virtual void OnDemolished() { throw new System.NotImplementedException(); }
        public virtual void OnCanclDemolish() { throw new System.NotImplementedException(); }
    }
}