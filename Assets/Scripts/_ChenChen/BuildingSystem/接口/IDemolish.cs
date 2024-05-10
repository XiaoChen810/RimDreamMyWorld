

namespace ChenChen_BuildingSystem
{
    /// <summary>
    /// 可拆卸的
    /// </summary>
    internal interface IDemolish
    {
        //int Workload { get; set; }
        void OnMarkDemolish();
        void OnDemolish(int value);
        void OnDemolished();
    }
}
