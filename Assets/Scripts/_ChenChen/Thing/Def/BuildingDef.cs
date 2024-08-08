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
        public string Description;

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
        public List<Need> RequiredMaterials = new List<Need>();

        [Tooltip("�����ߴ�")]
        public Vector2Int Size = Vector2Int.one;

        //----------------------------------------------------------------------
        [Header("��ѡ����")]

        [Tooltip("������ɵ���Ƭ")]
        public TileBase TileBase;

        [Tooltip("�Ƿ����ϰ���")]
        public bool IsObstacle;

        [Tooltip("�Ƿ�Ӱ�콨��")]
        public bool IsEffectBuild = false;

    }
}