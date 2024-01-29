using UnityEngine;

namespace 建筑系统
{
    [System.Serializable]
    public abstract class BuildingBlueprintBase : MonoBehaviour, IBlueprint
    {
        public string Name;
        public BlueprintData _BlueprintData {  get; protected set; }
        public float _workloadRemainder {  get; protected set; }
        private void OnEnable()
        {
            _BlueprintData = BuildingSystemManager.Instance.GetData(Name);
            _workloadRemainder = _BlueprintData.Workload;
        }

        // 实现接口中定义的属性和方法
        public abstract void Placed();
        public abstract void Build(float thisWorkload);
        public abstract void Complete();
        public abstract void Cancel();
    }
}