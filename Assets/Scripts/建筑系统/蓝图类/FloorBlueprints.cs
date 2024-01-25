using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace 建筑系统
{
    public class FloorBlueprints : BuildingBlueprintBase
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
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(1, 1, 1, 1f);

            BuildingSystemManager.Instance.RemoveTask(this);
        }

        public override void Placed()
        {
            Debug.Log("Placed");
            // 变成半透明，表示还未完成
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(1, 1, 1, 0.5f);

            // 添加到建筑管理系统中
            BuildingSystemManager.Instance.AddTask(this);
        }
    }
}