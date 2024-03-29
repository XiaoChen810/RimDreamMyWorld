
namespace ChenChen_BuildingSystem
{
    public interface IBlueprint
    {
        // 放置蓝图时调用的方法
        void Placed();

        // 完成建造时调用的方法
        void Complete();

        // 根据工作量执行建造的方法
        void Build(int thisWorkload);

        // 取消建造时调用的方法
        void Cancel();

        // 中断建造时调用的方法
        void Interpret();
    }
}