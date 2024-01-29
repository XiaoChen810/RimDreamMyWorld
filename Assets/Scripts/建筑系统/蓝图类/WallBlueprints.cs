using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ����ϵͳ
{

    /// <summary>
    ///  ǽ����ͼ����
    /// </summary>
    public class WallBlueprints : BuildingBlueprintBase
    {
        /*  
         *  Placed�������õ�Ŀ��㣬Ȼ����ӵ����������
         *  Build�������ڼ��ٹ�����
         *  Complete�����������ʱ��������Ϊ0���ҵ���Ӧ����Ƭ��ͼ�����϶�Ӧ����Ƭ�����ʱҲɾ����ͼ��Object
         *  Cancel�������ڵ�ȡ������ʱ��ɾ����ͼ��Object
        */

        public override void Placed()
        {
            Debug.Log("Placed");
            // ��ɰ�͸������ʾ��δ���
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(1, 1, 1, 0.5f);

            // ��ӵ���������ϵͳ��
            BuildingSystemManager.Instance.AddTask(this);
        }

        public override void Build(float thisWorkload)
        {
            Debug.Log("Build");
            _workloadRemainder -= thisWorkload;
        }

        public override void Complete()
        {
            Debug.Log("Complete");
            // ����Ƭ��ͼ������Ƭ
            Tilemap WallTilemap = BuildingSystemManager.Instance._BuildingSystem.WallTilemap;
            Vector3Int completePos = WallTilemap.WorldToCell(transform.position);
            WallTilemap.SetTile(completePos, _BlueprintData.TileBase);

            BuildingSystemManager.Instance.CompleteTask(this);
            Destroy(gameObject);
        }

        public override void Cancel()
        {
            Debug.Log("Cancel");

            BuildingSystemManager.Instance.CanelTask(this);
            Destroy(gameObject);
        }
    }
}