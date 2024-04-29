

namespace ChenChen_BuildingSystem
{
    /// <summary>
    /// 可拆卸的
    /// </summary>
    internal interface IDismantlable
    {
        //int Workload { get; set; }
        void OnMarkDemolish();
        void OnDemolish(int value);
        void OnDemolished();
    }
}
