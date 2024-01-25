using UnityEngine;

namespace 建筑系统
{
    [System.Serializable]
    public abstract class BuildingBlueprintBase : MonoBehaviour, IBlueprint
    {
        [SerializeField] protected string Name;
        [SerializeField] protected BlueprintData _BlueprintData;
        [SerializeField] protected int _workload;

        private void OnEnable()
        {
            _BlueprintData = BuildingSystemManager.Instance.GetData(Name);
            _workload = _BlueprintData.Workload;
        }

        // 实现接口中定义的属性和方法
        public abstract void Placed();
        public abstract void Complete();
        public abstract void Build(int thisWorkload);
        public abstract void Cancel();
    }
}