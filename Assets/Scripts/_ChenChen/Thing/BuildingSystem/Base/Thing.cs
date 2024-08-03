using UnityEngine;
using ChenChen_Map;
using ChenChen_UI;
using UnityEngine.Tilemaps;

namespace ChenChen_Thing
{
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class Thing : PermissionBase, IBlueprint, IDemolish, IDetailView 
    {
        [Header("物品属性")]
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

        /// <summary>
        /// 物品自身SpriteRenderer
        /// </summary>
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


        protected int _workload = -1;
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
                _lifeState = change;
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
            name = name.Replace("(Clone)", "").Trim();
            name = name.Replace("_Prefab", "").Trim();
            tag = "Thing";
            MapName = MapManager.Instance.CurrentMapName;

            ColliderSelf = GetComponent<BoxCollider2D>();
            ColliderSelf.isTrigger = true;

            SR = GetComponentInChildren<SpriteRenderer>();
            SR.sortingLayerName = "Middle";
            SR.sortingOrder = -(int)transform.position.y;

            centerTexture = new Texture2D(1, 1);
            centerTexture.SetPixel(0, 0, centerColor);
            centerTexture.Apply();

            outlineTexture = new Texture2D(1, 1);
            outlineTexture.SetPixel(0, 0, outlineColor);
            outlineTexture.Apply();

            OnPlaced();
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

        protected virtual void OnDestroy()
        {
            if (!Application.isPlaying) return;

            if (ThingSystemManager.Instance.RemoveThing(this.gameObject))
            {
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
                    panel.RemovePanel(panel.GetTopPanel());
                }

                if (Def.IsObstacle)
                {
                    if (AstarPath.active != null)
                    {
                        Bounds bounds = ColliderSelf.bounds;
                        AstarPath.active.UpdateGraphs(bounds);
                    }
                }
            }
        }


        public virtual bool CanBuildHere()
        {
            Collider2D collider = ColliderSelf;
            Bounds bounds = collider.bounds;
            Vector2 center = bounds.center;
            Collider2D[] colliders = Physics2D.OverlapBoxAll(center, bounds.size, 0f);

            foreach(var coll in colliders)
            {
                if (coll.gameObject == this.gameObject) continue;
                if (coll.CompareTag("Pawn")) continue;
                if (coll.CompareTag("Floor")) continue;
                return false;              
            }

            return true;
        }

        public virtual void MarkToBuild()
        {
            ChangeLifeState(BuildingLifeStateType.MarkBuilding);
        }

        public virtual void MarkToDemolish()
        {
            ChangeLifeState(BuildingLifeStateType.MarkDemolished);
        }

        #region - IBlueprint -
        public virtual void OnPlaced()
        {
            CurDurability = MaxDurability;
        }

        public virtual void OnMarkBuild()
        {
            SR.color = new Color(1, 1, 1, 0f);
            DrawOutline_Collider = true;
            // 如果所需工作量为0，直接完成
            if (WorkloadBuilt <= 0)
            {
                ChangeLifeState(BuildingLifeStateType.FinishedBuilding);
                return;
            }
            // 只有第一次标记建造，才设置工作量
            if (Workload <= -1)
            {
                Workload = WorkloadBuilt;
            }
        }

        public virtual void OnBuild(int value)
        {
            Workload -= value;
            if (Workload <= 0)
            {
                Workload = 0;
                ChangeLifeState(BuildingLifeStateType.FinishedBuilding);
            }
        }

        public virtual void OnCompleteBuild()
        {
            // 在瓦片地图设置瓦片
            if (Def.TileBase != null)
            {
                if (MapManager.Instance.TryGetTilemap(_tilemapName, true, out Tilemap tilemap))
                {
                    tilemap.SetTile(StaticFuction.VectorTransToInt(transform.position), Def.TileBase);
                    SR.color = new Color(1, 1, 1, 0f);
                }
                else
                {
                    Debug.LogError("Error in set tile");
                }
            }
            else
            {
                SR.color = new Color(1, 1, 1, 1f);
            }

            // 如果是障碍物,给所在的地图的该位置设置存在障碍物,设置碰撞体
            if (Def.IsObstacle)
            {
                ColliderSelf.isTrigger = false;
                gameObject.layer = 8; //"Obstacle"
                if (AstarPath.active != null)
                {
                    Bounds bounds = ColliderSelf.bounds;
                    AstarPath.active.UpdateGraphs(bounds);
                }
                else
                {
                    Debug.LogError("AstarPath.active is null");
                }
            }
            CanDemolish = true;
            DrawOutline_Collider = false;
        }

        public virtual void OnCancelBuild()
        {
            Destroy(gameObject);
        }

        public virtual void OnInterpretBuild()
        {
            ChangeLifeState(BuildingLifeStateType.MarkBuilding);
        }
        #endregion

        #region - IDemolish -

        private GameObject markDemolishIcon = null;

        public virtual void OnDemolish(int value)
        {
            CurDurability -= value;
            if (CurDurability <= 0)
            {
                CurDurability = 0;
                ChangeLifeState(BuildingLifeStateType.FinishedDemolished);
            }
            // 更新工作量
            Workload = CurDurability;
        }

        public virtual void OnMarkDemolish()
        {
            // 拆除的工作量是当前耐久
            Workload = CurDurability;
            if (CurDurability <= 0)
            {
                ChangeLifeState(BuildingLifeStateType.FinishedDemolished);
            }
            markDemolishIcon = GameObject.Instantiate(Resources.Load("Prefabs/ThingDef/MarkDemolishIcon")) as GameObject;
            markDemolishIcon.transform.position = this.transform.position;
        }

        public virtual void OnDemolished()
        {
            if (markDemolishIcon != null)
                Destroy(markDemolishIcon);
            Destroy(gameObject);
        }

        public virtual void OnCanclDemolish()
        {
            // 回到刚建造完的时候
            ChangeLifeState(BuildingLifeStateType.FinishedBuilding);

            if (markDemolishIcon != null)
                Destroy(markDemolishIcon);
        }
        #endregion
    }
}