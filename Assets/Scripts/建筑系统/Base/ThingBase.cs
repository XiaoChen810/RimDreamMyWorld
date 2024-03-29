using UnityEngine;
using ChenChen_MapGenerator;

namespace ChenChen_BuildingSystem
{
    [System.Serializable]
    public abstract class ThingBase : MonoBehaviour, IBlueprint, IDismantlable
    {
        public BlueprintData Data;
        public string MapName;
        public bool IsDismantlable;

        public int WorkloadBuilt
        {
            get { return Data.Workload; }
        }

        public int WorkloadDemolished
        {
            get { return Mathf.CeilToInt(Data.Workload * 0.5f); }
        }

        public int MaxDurability
        {
            get { return Data.Durability; }
        }

        public int CurDurability;


        [SerializeField] protected int _needWorkload;
        public int NeedWorkload
        {
            get
            {
                return _needWorkload;
            }
            set
            {
                _needWorkload = value > 0 ? value : 0;
            }
        }

        public BoxCollider2D _myCollider { get; protected set; }


        protected virtual void OnEnable()
        {
            gameObject.name = gameObject.name.Replace("(Clone)", "");
            MapName = MapManager.Instance.CurrentMapName;
            CurDurability = MaxDurability;
            _myCollider = GetComponent<BoxCollider2D>();
        }

        // 实现接口中定义的属性和方法
        public abstract void Placed();
        public abstract void Build(int thisWorkload);
        public abstract void Complete();
        public abstract void Cancel();
        public abstract void Interpret();
        public abstract void OnMarkDemolish();
        public abstract void Demolish(int value);
        public abstract void OnDemolished();
    }
}