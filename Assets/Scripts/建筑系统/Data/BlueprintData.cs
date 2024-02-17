using UnityEngine;
using UnityEngine.Tilemaps;

namespace MyBuildingSystem
{
    public enum BlueprintType
    {
        Floor,Wall,Building,Furniture,Other
    }

    [CreateAssetMenu(menuName = "��ͼ����", fileName = "��ͼ����", order = 0)]
    public class BlueprintData : ScriptableObject
    {
        public string Name;

        [Header("����")]
        public BlueprintType Type;

        [Header("ռ�ݸ��ӵĳ��Ϳ�")]
        public int BuildingWidth;
        public int BuildingHeight;

        [Header("�������蹤����")]
        public int Workload;

        [Header("Ԥ��ͼ")]
        public Sprite PreviewSprite;

        [Header("�Ƿ�������Ϸ������Ŷ���")]
        public bool Stackable;

        [Header("������ɵĽ�����Ԥ����")]
        public GameObject Prefab;

        [Header("������ɵ���Ƭ")]
        public TileBase TileBase;
    }
}