using UnityEngine;

public interface IBlueprint
{
    BlueprintData BlueprintData { get; set; }

    // ������ͼʱ���õķ���
    void Placed();

    // ��ɽ���ʱ���õķ���
    void Complete();

    // ���ݹ�����ִ�н���ķ���
    void Build(int thisWorkload);

    // ȡ������ʱ���õķ���
    void Cancel();
}

