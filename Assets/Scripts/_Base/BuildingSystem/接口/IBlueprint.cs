
namespace ChenChen_BuildingSystem
{
    public interface IBlueprint
    {
        // 放置蓝图时调用的方法
        void OnPlaced(BuildingLifeStateType initial_State);
        // 标记建造时调用的方法
        void OnMarkBuild();
        // 执行建造时的方法
        void OnBuild(int value);
        // 完成建造时调用的方法
        void OnComplete();
        // 取消建造时调用的方法
        void OnCancel();
        // 中断建造时调用的方法
        void OnInterpret();
    }
}