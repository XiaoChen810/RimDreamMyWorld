
namespace ChenChen_BuildingSystem
{
    public interface IBlueprint
    {
        // ������ͼʱ���õķ���
        void OnPlaced(BuildingLifeStateType initial_State, string mapName);
        // ��ǽ���ʱ���õķ���
        void OnMarkBuild();
        // ִ�н���ʱ�ķ���
        void OnBuild(int value);
        // ��ɽ���ʱ���õķ���
        void OnCompleteBuild();
        // ȡ������ʱ���õķ���
        void OnCancelBuild();
        // �жϽ���ʱ���õķ���
        void OnInterpretBuild();
    }
}