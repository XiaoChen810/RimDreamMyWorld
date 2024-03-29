

namespace ChenChen_BuildingSystem
{
    /// <summary>
    /// 可拆卸的
    /// </summary>
    internal interface IDismantlable
    {
        int NeedWorkload { get; set; }
        void OnMarkDemolish();
        void Demolish(int value);
        void OnDemolished();
    }
}
