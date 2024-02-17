using MyBuildingSystem;
using UnityEngine;

public interface IBlueprint
{
    // ������ͼʱ���õķ���
    void Placed();

    // ��ɽ���ʱ���õķ���
    void Complete();

    // ���ݹ�����ִ�н���ķ���
    void Build(float thisWorkload);

    // ȡ������ʱ���õķ���
    void Cancel();
}

