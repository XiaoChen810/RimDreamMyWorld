
namespace ChenChen_Thing
{
    public interface IBlueprint
    {
        // ������ͼʱ���õķ���
        void OnPlaced();
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