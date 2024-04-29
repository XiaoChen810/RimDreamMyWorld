using UnityEngine;
using UnityEngine.Tilemaps;

namespace ChenChen_BuildingSystem
{
    public enum ThingType
    {
        Floor,Wall,Building,Furniture,Other,Tree
    }

    /// <summary>
    /// ��Ʒ����
    /// </summary>
    [CreateAssetMenu(menuName = "��ͼ����", fileName = "��ͼ����", order = 0)]
    public class ThingDef : ScriptableObject
    {
        public string Name;

        [Header("����")]
        public ThingType Type;

        [Header("�������蹤����")]
        public int Workload;

        [Header("Ԥ��ͼ")]
        public Sprite PreviewSprite;

        [Header("�Ƿ����ϰ���")]
        public bool IsObstacle;

        [Header("������ɵĽ�����Ԥ����")]
        public GameObject Prefab;

        [Header("������ɵ���Ƭ")]
        public TileBase TileBase;

        [Header("�;ö�")]
        public int Durability;

        [Header("��������")]
        public Vector3 offset;

    }
}