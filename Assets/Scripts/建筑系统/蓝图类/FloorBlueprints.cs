using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace 建筑系统
{
    public class FloorBlueprints : BuildingBlueprintBase
    {
        /*  
        *  Placed函数放置到目标点，然后添加到建造队列中
        *  Build函数用于减少工作量
        *  Complete函数用于完成时（工作量为0）变成完成时的状态
        *  Cancel函数用于当取消建造时，删掉蓝图的Object
        */
        public override void Placed()
        {
            
            // 变成半透明，表示还未完成
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(1, 1, 1, 0.5f);

            // 添加到建筑管理系统中
            BuildingSystemManager.Instance.AddTask(this);
        }

        public override void Build(float thisWorkload)
        {           
            _workloadRemainder -= thisWorkload;
            Debug.Log("正在建造,剩余" + _workloadRemainder);
        }


        public override void Complete()
        {
            
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(1, 1, 1, 1f);

            BuildingSystemManager.Instance.CompleteTask(this);
        }

        public override void Cancel()
        {
            
            Destroy(this.gameObject);
            BuildingSystemManager.Instance.CanelTask(this);
        }

    }
}