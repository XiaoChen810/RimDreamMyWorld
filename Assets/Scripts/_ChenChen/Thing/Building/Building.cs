using ChenChen_Map;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using ChenChen_UI;

namespace ChenChen_Thing
{
    public class Building : Thing, IBuilding, IDemolish, IStorage
    {
        // 物品剩余工作量
        protected int _workload = -1;
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

        [Header("建筑属性")]
        public BuildingDef Def;

        #region - State -
        [SerializeField] protected BuildingLifeStateType _lifeState = 0;       
        public BuildingLifeStateType LifeState => _lifeState;
        private void ChangeLifeState(BuildingLifeStateType change)
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
        #endregion

        #region - 绘制效果 -

        [SerializeField] protected string _tilemapName = "Building"; // 放置瓦片（如果有）的瓦片地图名

        // 绘制GUI
        public bool DrawOutline_Collider;    // 根据Box Collider2D画
        private Color centerColor = new Color(0f, 0f, 1f, 0f); // 透明蓝色
        private Color outlineColor = Color.white; // 纯白色
        private float outlineWidth = 2f;

        private Texture2D centerTexture;
        private Texture2D outlineTexture;

        protected virtual Action<DetailViewPanel> DetailViewOverrideContentAction()
        {
            return (DetailViewPanel panel) =>
            {
                List<String> content = new List<String>();
                content.Add($"耐久度: {this.Durability} / {this.MaxDurability}");
                content.Add($"使用者: {(this.UserPawn != null ? this.UserPawn.name : null)}");
                if (this.Workload > 0)
                {
                    content.Add($"剩余工作量: {this.Workload}");
                }
                panel.SetView(this.Def.DefName, content);
                if (this.LifeState == BuildingLifeStateType.MarkBuilding)
                {
                    panel.RemoveAllButton();
                    panel.SetButton("取消", () =>
                    {
                        this.OnCancelBuild();
                    });
                }
                if (this.LifeState == BuildingLifeStateType.MarkDemolished)
                {
                    panel.RemoveAllButton();
                    panel.SetButton("取消", () =>
                    {
                        this.OnCanclDemolish();
                    });
                }
                if (this.LifeState == BuildingLifeStateType.FinishedBuilding)
                {
                    panel.RemoveAllButton();
                    panel.SetButton("拆除", () =>
                    {
                        this.MarkToDemolish();
                    });
                }
            };
        }

        protected override void Start()
        {
            base.Start();

            centerTexture = new Texture2D(1, 1);
            centerTexture.SetPixel(0, 0, centerColor);
            centerTexture.Apply();

            outlineTexture = new Texture2D(1, 1);
            outlineTexture.SetPixel(0, 0, outlineColor);
            outlineTexture.Apply();

            DetailView.OverrideContentAction = (DetailViewOverrideContentAction());
        }

        private void OnGUI()
        {
            if (GameManager.Instance.Mode_Cineme) return;
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

        protected override void OnEnable()
        {
            base.OnEnable();
            OnPlaced();
        }

        protected override void OnDestroy()
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

        #endregion

        #region - Public -

        public virtual bool CanBuildHere()
        {
            Collider2D collider = ColliderSelf;
            Bounds bounds = collider.bounds;
            Vector2 center = bounds.center;
            Collider2D[] colliders = Physics2D.OverlapBoxAll(center, bounds.size, 0f);

            foreach (var coll in colliders)
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
        #endregion

        #region - IBuilding -
        public int Workload_Construction
        {
            get
            {
                if (GameManager.Instance.IsGodMode)
                {
                    return 0;
                }
                return Def.Workload;
            }
        }

        public virtual void OnPlaced()
        {
            //Debug.Log($"放置一个建筑：{name}");
        }

        public virtual void OnMarkBuild()
        {
            SR.color = new Color(1, 1, 1, 0f);
            DrawOutline_Collider = true;
            // 如果所需工作量为0，直接完成
            if (Workload_Construction <= 0)
            {
                ChangeLifeState(BuildingLifeStateType.FinishedBuilding);
                return;
            }
            // 只有第一次标记建造，才设置工作量
            if (Workload <= -1)
            {
                Workload = Workload_Construction;
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

        public int Workload_Demolition
        {
            get
            {
                if (GameManager.Instance.IsGodMode)
                {
                    return 0;
                }
                return Def.Durability;
            }
        }

        private GameObject markDemolishIcon = null;

        public virtual void OnDemolish(int value)
        {
            Durability -= value;
            if (Durability <= 0)
            {
                Durability = 0;
                ChangeLifeState(BuildingLifeStateType.FinishedDemolished);
            }
            // 更新工作量
            Workload = Workload_Demolition;
        }

        public virtual void OnMarkDemolish()
        {
            // 拆除的工作量是当前耐久
            Workload = Workload_Demolition;
            if (Durability <= 0)
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
