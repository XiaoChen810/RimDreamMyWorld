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
        [Header("��������")]
        public string DefName;
        public ThingType Type;

        [Tooltip("�������蹤����")]
        public int Workload;

        [Tooltip("�;ö�")]
        public int Durability;

        [Tooltip("Ԥ��ͼ")]
        public Sprite PreviewSprite;

        [Tooltip("������ɵĽ�����Ԥ����")]
        public GameObject Prefab;

        [Tooltip("����λ��ƫ����")]
        public Vector2 Offset = new Vector2(0.5f, 0.5f);

        //----------------------------------------------------------------------
        [Header("��ѡ����")]

        [Tooltip("�Ƿ����ϰ���")]
        public bool IsObstacle;

        [Tooltip("������ɵ���Ƭ")]
        public TileBase TileBase;

        [Tooltip("�Ƿ�ѡ������ʵ��")]
        public bool IsNotInstancing;

        [Tooltip("��������")]
        public bool CanRotation;

    }
}