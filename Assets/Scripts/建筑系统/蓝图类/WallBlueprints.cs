using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using MyMapGenerate;

namespace MyBuildingSystem
{

    /// <summary>
    ///  ǽ����ͼ����
    /// </summary>
    public class WallBlueprints : BlueprintBase
    {
        /*  
         *  Placed�������õ�Ŀ��㣬Ȼ����ӵ����������
         *  Build�������ڼ��ٹ�����
         *  Complete�����������ʱ��������Ϊ0���ҵ���Ӧ����Ƭ��ͼ�����϶�Ӧ����Ƭ�����ʱҲɾ����ͼ��Object
         *  Cancel�������ڵ�ȡ������ʱ��ɾ����ͼ��Object
        */
        private Tilemap _wallTilemap;
        private Vector3Int _completePos;

        public override void Placed()
        {
            // ��ɰ�͸������ʾ��δ���
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(1, 1, 1, 0.5f);

            // ��ӵ���������ϵͳ��
            BuildingSystemManager.Instance.AddTask(this);

            _wallTilemap = BuildingSystemManager.Instance.BlueprintTool.WallTilemap;
            _completePos = _wallTilemap.WorldToCell(transform.position);
            // �����ڵĵ�ͼ�ĸ�λ�������Ѵ��ڽ�����
            MapManager.Instance.SetMapDataWalkable(_myMapName, _completePos, false);
        }

        public override void Build(float thisWorkload)
        {
            _workloadAlready -= thisWorkload;
        }

        public override void Complete()
        {
            // ����Ƭ��ͼ������Ƭ
            _wallTilemap.SetTile(_completePos, _BlueprintData.TileBase);

            // �����ڵĵ�ͼ��Ѱ·�㷨��������ϰ�
            MapManager.Instance.SetNodeWalkable(_myMapName, _completePos,false);
            BuildingSystemManager.Instance.CompleteTask(this);
            BuildingSystemManager.Instance.WallHashSet.Add(_completePos);
            Destroy(gameObject);
        }

        public override void Cancel()
        {
            BuildingSystemManager.Instance.CanelTask(this);
            // �����ڵĵ�ͼ�ĸ�λ�ó����Ѵ��ڽ�����
            MapManager.Instance.SetMapDataWalkable(_myMapName, _completePos, true);
            Destroy(gameObject);
        }
    }
}