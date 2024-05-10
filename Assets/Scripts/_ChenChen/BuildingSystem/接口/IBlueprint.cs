
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
        void OnComplete();
        // ȡ������ʱ���õķ���
        void OnCancel();
        // �жϽ���ʱ���õķ���
        void OnInterpret();
    }
}