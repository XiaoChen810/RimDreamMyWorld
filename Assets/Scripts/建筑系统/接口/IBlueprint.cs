
namespace ChenChen_BuildingSystem
{
    public interface IBlueprint
    {
        // ������ͼʱ���õķ���
        void Placed();

        // ��ɽ���ʱ���õķ���
        void Complete();

        // ���ݹ�����ִ�н���ķ���
        void Build(int thisWorkload);

        // ȡ������ʱ���õķ���
        void Cancel();

        // �жϽ���ʱ���õķ���
        void Interpret();
    }
}