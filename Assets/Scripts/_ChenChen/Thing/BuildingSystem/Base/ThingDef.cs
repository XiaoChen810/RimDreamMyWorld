using UnityEngine;
using UnityEngine.Tilemaps;

namespace ChenChen_Thing
{
    /// <summary>
    /// ��Ʒ����
    /// </summary>
    [CreateAssetMenu(menuName = "����/����", fileName = "ThingDef", order = 0)]
    public class ThingDef : ScriptableObject
    {
        public string DefName;

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
        public Vector2 Offset;
        public bool CanRotation;

    }
}