using UnityEngine;
using ChenChen_MapGenerator;

namespace ChenChen_BuildingSystem
{
    [System.Serializable]
    public abstract class BlueprintBase : MonoBehaviour, IBlueprint
    {
        public BlueprintData Data;
        public string MapName;
        public float WorkLoad;

        public BoxCollider2D _myCollider { get; protected set; }

        protected virtual void OnEnable()
        {
            gameObject.name = gameObject.name.Replace("(Clone)", "");
            WorkLoad = Data.Workload;
            MapName = MapManager.Instance.CurrentMapName;
            _myCollider = GetComponent<BoxCollider2D>();
        }

        // 实现接口中定义的属性和方法
        public abstract void Placed();
        public abstract void Build(float thisWorkload);
        public abstract void Complete();
        public abstract void Cancel();
        public abstract void Interpret();

    }
}