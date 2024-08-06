

namespace ChenChen_Thing
{
    /// <summary>
    /// 可拆卸的
    /// </summary>
    public interface IDemolish
    {
        int Workload_Demolition { get; }
        void OnMarkDemolish();
        void OnDemolish(int value);
        void OnDemolished();
        void OnCanclDemolish();
    }
}
