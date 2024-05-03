using UnityEngine;
using ChenChen_MapGenerator;
using ChenChen_AI;

namespace ChenChen_BuildingSystem
{
    [System.Serializable]
    public abstract class ThingBase : MonoBehaviour, IBlueprint, IDismantlable, IDetailView
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
        public bool IsDismantlable { get; protected set; }

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
                    Debug.LogWarning($"The building life state has been {value}");
                }
            }
        }

        // 使用的棋子
        protected Pawn _theUsingPawn;
        public Pawn TheUsingPawn
        {
            get
            {
                return _theUsingPawn;
            }
            protected set
            {
                _theUsingPawn = value;
            }
        }

        // 是否被使用
        protected bool _isUsed;
        public bool IsUsed
        {
            get
            {
                return _isUsed;
            }
            protected set
            {
                _isUsed = value;
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
        protected virtual void OnEnable()
        {
            gameObject.name = gameObject.name.Replace("(Clone)", "");
            CurDurability = MaxDurability;
            ColliderSelf = GetComponent<BoxCollider2D>();
            _detailView = gameObject.AddComponent<DetailView_Thing>();
        }
        // 实现接口中定义的属性和方法
        public abstract void OnPlaced(BuildingLifeStateType initial_State, string mapName);
        public abstract void OnMarkBuild();
        public abstract void OnBuild(int value);
        public abstract void OnComplete();
        public abstract void OnCancel();
        public abstract void OnInterpret();
        public abstract void OnMarkDemolish();
        public abstract void OnDemolish(int value);
        public abstract void OnDemolished();
    }
}