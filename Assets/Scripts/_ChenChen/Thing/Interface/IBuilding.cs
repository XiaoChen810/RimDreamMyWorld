
using Mono.Cecil.Cil;

namespace ChenChen_Thing
{
    /// <summary>
    /// �ɽ����
    /// </summary>
    public interface IBuilding
    {
        int Workload_Construction { get;}
        void OnPlaced();
        void OnMarkBuild();
        void OnBuild(int value);
        void OnCompleteBuild();
        void OnCancelBuild();
        void OnInterpretBuild();
    }
}