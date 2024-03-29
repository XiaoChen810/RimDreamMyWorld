using UnityEngine;
using UnityEngine.Tilemaps;

namespace ChenChen_BuildingSystem
{
    public enum BlueprintType
    {
        Floor,Wall,Building,Furniture,Other
    }

    /// <summary>
    /// The data for thing
    /// </summary>
    [CreateAssetMenu(menuName = "��ͼ����", fileName = "��ͼ����", order = 0)]
    public class BlueprintData : ScriptableObject
    {
        public string Name;

        [Header("����")]
        public BlueprintType Type;

        //[Header("ռ�ݸ��ӵĳ��Ϳ�")]
        //public int BuildingWidth;
        //public int BuildingHeight;

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

    }
}