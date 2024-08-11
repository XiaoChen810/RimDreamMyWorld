using ChenChen_Map;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using ChenChen_UI;
using ChenChen_Core;
using ChenChen_AI;

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

        // 建造 所 需要的材料
        private List<Need> requiredMaterialList = new List<Need>();
        // 建造 还 需要的材料
        public IReadOnlyList<(string, int)> RequiredMaterialList
        {
            get
            {
                List<(string, int)> res = new List<(string, int)>();
                foreach (Need requiredMaterial in requiredMaterialList)
                {
                    int had = Bag.ContainsKey(requiredMaterial.label) ? Bag[requiredMaterial.label] : 0;
                    int required = requiredMaterial.numbers;
                    int need = required - had;
                    if (need != 0)
                    {
                        res.Add((requiredMaterial.label, need));
                    }
                }
                return res;
            }
        }
        // 建造需要的材料已经准备完全
        public bool RequiredMaterialsLoadFull
        {
            get
            {
                if (GameManager.Instance.IsGodMode)
                {
                    return true;
                }
                foreach (Need need in requiredMaterialList)
                {
                    if (Bag.ContainsKey(need.label) && Bag[need.label] == need.numbers)
                    {
                        continue;
                    }
                    return false;
                }
                return true;
            }
        }

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

        private static readonly string _tilemapName = "Building"; 

        protected virtual void DetailViewOverrideContentAction(DetailViewPanel panel)
        {
            if (panel == null) return;

            // String Content
            List<String> content = new List<String>();
            InitDetailViewContent(content);
            panel.SetView(this.Def.DefName, content);

            // Button
            InitDetailViewButton(panel);
        }

        protected void InitDetailViewContent(List<string> content)
        {
            content.Add($"耐久度: {this.Durability} / {this.MaxDurability}");
            content.Add($"使用者: {(this.UserPawn != null ? this.UserPawn.name : null)}");
            if (this.Workload > 0)
            {
                content.Add($"剩余工作量: {this.Workload}");
            }
            if (!this.RequiredMaterialsLoadFull)
            {
                foreach (Need need in requiredMaterialList)
                {
                    int a = Bag.ContainsKey(need.label) ? Bag[need.label] : 0;
                    int b = need.numbers;
                    content.Add($"需要{need.name}: {a}/{b}");
                }
            }
        }

        protected void InitDetailViewButton(DetailViewPanel panel)
        {
            panel.RemoveAllButton();
            if (this.LifeState == BuildingLifeStateType.MarkBuilding)
            {              
                panel.SetButton("取消", () =>
                {
                    this.OnCancelBuild();
                });
            }
            if (this.LifeState == BuildingLifeStateType.MarkDemolished)
            {
                panel.SetButton("取消", () =>
                {
                    this.OnCanclDemolish();
                });
            }
            if (this.LifeState == BuildingLifeStateType.FinishedBuilding)
            {
                panel.SetButton("拆除", () =>
                {
                    this.MarkToDemolish();
                });
            }
        }

        protected override void Start()
        {
            base.Start();

            OnPlaced();

            DetailView.OverrideContentAction = DetailViewOverrideContentAction;
        }

        protected override void OnDestroy()
        {
            if (!Application.isPlaying) return;

            var thingSystemManager = ThingSystemManager.Instance;

            if (thingSystemManager != null && thingSystemManager.RemoveThing(this.gameObject))
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
            requiredMaterialList.Clear();
            foreach(var need in Def.RequiredMaterials)
            {
                var add = new Need()
                {
                    label = need.label,
                    numbers = need.numbers,
                };
                requiredMaterialList.Add(add);
            }
        }

        public virtual void OnMarkBuild()
        {
            SR.color = new Color(1, 1, 1, 0.7f);
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
                return Durability;
            }
        }

        private GameObject markDemolishIcon = null;

        public virtual void OnDemolish(int value)
        {
            Durability -= value;
            Workload = Workload_Demolition;
            if (Workload <= 0)
            {
                Durability = 0;
                ChangeLifeState(BuildingLifeStateType.FinishedDemolished);
            }
        }

        public virtual void OnMarkDemolish()
        {
            Workload = Workload_Demolition;
            if (Workload <= 0)
            {
                ChangeLifeState(BuildingLifeStateType.FinishedDemolished);
            }
            else
            {
                var obj = Resources.Load("Prefabs/ThingDef/MarkDemolishIcon") as GameObject;
                markDemolishIcon = Instantiate(obj);
                markDemolishIcon.transform.position = this.transform.position;
            }
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

        #region - IStorage -
        private Dictionary<string, int> bag = new Dictionary<string, int>();

        public Dictionary<string, int> Bag => bag;

        public void Put(string label, int num)
        {
            if (bag.ContainsKey(label))
            {
                bag[label] += num;
            }
            else
            {
                bag.Add(label, num);
            }
        }

        public int Get(string label, int num)
        {
            if (bag.ContainsKey(label))
            {
                int store = bag[label];
                if (store - num > 0)
                {
                    store -= num;
                    return num;
                }
                else
                {
                    bag.Remove(label);
                    return store;
                }
            }
            else
            {
                return 0;
            }
        }
        #endregion

        public override string CommandName => "优先建造";

        public override void CommandFunc(Pawn p)
        {
            if (LifeState == BuildingLifeStateType.MarkBuilding)
            {
                p.StateMachine.TryChangeState(new PawnJob_Building(p, new TargetPtr(this.gameObject)));
            }
        }
    }
}
