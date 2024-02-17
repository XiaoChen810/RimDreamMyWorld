using UnityEngine;
using MyMapGenerate;

namespace MyBuildingSystem
{
    [System.Serializable]
    public abstract class BlueprintBase : MonoBehaviour, IBlueprint
    {
        public BlueprintData _BlueprintData;
        public BoxCollider2D _myCollider { get; protected set; }
        public string _myMapName { get; protected set; }

        public float _workloadAlready;


        protected virtual void OnEnable()
        {
            gameObject.name = gameObject.name.Replace("(Clone)", "");
            _workloadAlready = _BlueprintData.Workload;
            _myMapName = MapManager.Instance.CurrentMapName;
            _myCollider = GetComponent<BoxCollider2D>();
        }

        // 实现接口中定义的属性和方法
        public abstract void Placed();
        public abstract void Build(float thisWorkload);
        public abstract void Complete();
        public abstract void Cancel();

        /// <summary>
        /// 添加该物体到BuildingSystemManager.Instance.BuildingHashSet
        /// </summary>
        protected virtual void End()
        {
            BuildingSystemManager.Instance.BuildingHashSet.Add(this.gameObject);
        }
    }
}