using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using MyMapGenerate;

namespace MyBuildingSystem
{

    /// <summary>
    ///  墙体蓝图的类
    /// </summary>
    public class WallBlueprints : BlueprintBase
    {
        /*  
         *  Placed函数放置到目标点，然后添加到建造队列中
         *  Build函数用于减少工作量
         *  Complete函数用于完成时（工作量为0）找到对应的瓦片地图，贴上对应的瓦片，完成时也删掉蓝图的Object
         *  Cancel函数用于当取消建造时，删掉蓝图的Object
        */
        private Tilemap _wallTilemap;
        private Vector3Int _completePos;

        public override void Placed()
        {
            // 变成半透明，表示还未完成
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(1, 1, 1, 0.5f);

            // 添加到建筑管理系统中
            BuildingSystemManager.Instance.AddTask(this);

            _wallTilemap = BuildingSystemManager.Instance.BlueprintTool.WallTilemap;
            _completePos = _wallTilemap.WorldToCell(transform.position);
            // 给所在的地图的该位置设置已存在建筑物
            MapManager.Instance.SetMapDataWalkable(_myMapName, _completePos, false);
        }

        public override void Build(float thisWorkload)
        {
            _workloadAlready -= thisWorkload;
        }

        public override void Complete()
        {
            // 在瓦片地图设置瓦片
            _wallTilemap.SetTile(_completePos, _BlueprintData.TileBase);

            // 给所在的地图的寻路算法网格添加障碍
            MapManager.Instance.SetNodeWalkable(_myMapName, _completePos,false);
            BuildingSystemManager.Instance.CompleteTask(this);
            BuildingSystemManager.Instance.WallHashSet.Add(_completePos);
            Destroy(gameObject);
        }

        public override void Cancel()
        {
            BuildingSystemManager.Instance.CanelTask(this);
            // 给所在的地图的该位置撤销已存在建筑物
            MapManager.Instance.SetMapDataWalkable(_myMapName, _completePos, true);
            Destroy(gameObject);
        }
    }
}