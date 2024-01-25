using ����ϵͳ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WallBlueprints : BuildingBlueprintBase
{
    public override void Build(int thisWorkload)
    {
        Debug.Log("Build");
        _workload -= thisWorkload;
    }

    public override void Cancel()
    {
        Debug.Log("Cancel");
        Destroy(this.gameObject);
    }

    public override void Complete()
    {
        Debug.Log("Complete");
        // ����Ƭ��ͼ������Ƭ
        BuildingSystem buildingSystem = GameObject.FindWithTag("BuildingSystem").GetComponent<BuildingSystem>();
        Tilemap WallTilemap = buildingSystem.WallTilemap;
        Vector3Int completePos = WallTilemap.WorldToCell(transform.position);
        WallTilemap.SetTile(completePos, _BlueprintData.TileBase);

        BuildingSystemManager.Instance.RemoveTask(this);
        Destroy(this.gameObject);
    }

    public override void Placed()
    {
        Debug.Log("Placed");
        // ��ɰ�͸������ʾ��δ���
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(1, 1, 1, 0.5f);

        // ��ӵ���������ϵͳ��
        BuildingSystemManager.Instance.AddTask(this);
    }
}
