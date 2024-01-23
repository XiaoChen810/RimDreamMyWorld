using UnityEngine;

[CreateAssetMenu(menuName = "��ͼ����", fileName = "��ͼ����", order = 0)]
public class BlueprintData : ScriptableObject
{
    [Header("ռ�ݸ��ӵĳ��Ϳ�")]
    public int BuildingWidth;
    public int BuildingHeight;

    [Header("�������蹤����")]
    public int Workload;

    [Header("Ԥ��ͼ")]
    public Sprite PreviewSprite;

    [Header("������ɵĽ�����Ԥ����")]
    public GameObject Prefab;
}
