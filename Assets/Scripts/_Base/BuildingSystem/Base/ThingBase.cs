using UnityEngine;
using ChenChen_MapGenerator;

namespace ChenChen_BuildingSystem
{
    [System.Serializable]
    public abstract class ThingBase : MonoBehaviour, IBlueprint, IDismantlable
    {
        /// <summary>
        /// 物品自身的定义
        /// </summary>
        public ThingDef Def;

        /// <summary>
        /// 物品所在的地图名
        /// </summary>
        public string MapName;

        /// <summary>
        /// 物品是可以拆除的
        /// </summary>
        public bool IsDismantlable;

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

        // 物品当前耐久度
        public int CurDurability;

        // 物品工作量
        [SerializeField] protected int _workload;
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

        // 物品碰撞体
        public BoxCollider2D ColliderSelf { get; protected set; }


        protected virtual void OnEnable()
        {
            gameObject.name = gameObject.name.Replace("(Clone)", "");
            MapName = MapManager.Instance.CurrentMapName;
            CurDurability = MaxDurability;
            ColliderSelf = GetComponent<BoxCollider2D>();
        }

        // 实现接口中定义的属性和方法
        public abstract void OnPlaced(BuildingLifeStateType initial_State = BuildingLifeStateType.None);
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