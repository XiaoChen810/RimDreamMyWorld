using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace 建筑系统
{

    /// <summary>
    ///  墙体蓝图的类
    /// </summary>
    public class WallBlueprints : BuildingBlueprintBase
    {
        /*  
         *  Placed函数放置到目标点，然后添加到建造队列中
         *  Build函数用于减少工作量
         *  Complete函数用于完成时（工作量为0）找到对应的瓦片地图，贴上对应的瓦片，完成时也删掉蓝图的Object
         *  Cancel函数用于当取消建造时，删掉蓝图的Object
        */

        public override void Placed()
        {
            Debug.Log("Placed");
            // 变成半透明，表示还未完成
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(1, 1, 1, 0.5f);

            // 添加到建筑管理系统中
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
            // 在瓦片地图设置瓦片
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