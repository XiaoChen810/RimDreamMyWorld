using ChenChen_Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ChenChen_Thing
{
    /// <summary>
    /// ��Ʒ����
    /// </summary>
    [CreateAssetMenu(menuName = "����/����", fileName = "ThingDef", order = 0)]
    public class BuildingDef : ScriptableObject
    {
        [Header("��������")]
        public string DefName;
        public BuildingType Type;

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

        [Tooltip("�����������")]
        public List<Need> RequiredMaterials;

        //----------------------------------------------------------------------
        [Header("��ѡ����")]

        [Tooltip("�Ƿ����ϰ���")]
        public bool IsObstacle;

        [Tooltip("������ɵ���Ƭ")]
        public TileBase TileBase;

        [Tooltip("�Ƿ�ѡ������ʵ��")]
        public bool IsNotInstancing;

        [Tooltip("������ת")]
        public bool CanRotation;

    }
}