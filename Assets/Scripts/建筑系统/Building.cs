using ChenChen_MapGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ChenChen_BuildingSystem
{
    public class Building : BlueprintBase
    {
        private Tilemap _wallTilemap;
        private Vector3Int _completePos;
        public bool isCompletely;

        /// <summary>
        /// 判断是否已经有棋子在使用
        /// </summary>
        [SerializeField] protected Pawn TheUsingPawn;
        [SerializeField] protected bool isUsed;

        public bool IsUsed
        {
            get
            {
                return isUsed;
            }
            protected set
            {
                isUsed = value;
            }
        }

        public override void Placed()
        {
            _wallTilemap = MapManager.Instance.GetChildObject("Building").GetComponent<Tilemap>();
            _completePos = _wallTilemap.WorldToCell(transform.position);

            // 变成半透明，表示还未完成
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(1, 1, 1, 0.5f);

            // 添加到建筑管理系统中
            if(WorkLoad <= 0)
            {
                BuildingSystemManager.Instance.AddBuildingListDone(this.gameObject);
            }
            else
            {
                BuildingSystemManager.Instance.AddTask(this.gameObject);
            }

            // 设置碰撞体
            if(TryGetComponent<Collider2D>(out Collider2D coll))
            {
                if(coll != null)
                {
                    GetComponent<Collider2D>().isTrigger = true;
                }
                else
                {
                    coll = gameObject.AddComponent<BoxCollider2D>();  
                    coll.isTrigger = true;
                }
            }
        }

        public override void Build(float thisWorkload)
        {
            WorkLoad -= thisWorkload;
        }

        public override void Complete()
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();

            // 在瓦片地图设置瓦片
            if (Data.TileBase != null)
            {
                _wallTilemap.SetTile(_completePos, Data.TileBase);
                sr.color = new Color(1, 1, 1, 0f);
            }
            else
            {
                sr.color = new Color(1, 1, 1, 1f);
            }

            // 如果是障碍物，则设置碰撞体,给所在的地图的该位置设置存在障碍物
            if (Data.IsObstacle)
            {
                MapManager.Instance.AddToObstaclesList(MapName, _completePos);
                if (Data.IsObstacle) GetComponent<Collider2D>().isTrigger = false;
            }

            // 在BuildingSystemManager上完成建造
            BuildingSystemManager.Instance.CompleteTask(this.gameObject);

            isCompletely = true;
        }

        public override void Cancel()
        {
            BuildingSystemManager.Instance.CanelTask(this.gameObject);
            Destroy(gameObject);
        }

        public override void Interpret()
        {
            BuildingSystemManager.Instance.InterpretTask(this.gameObject);
        }

        public virtual void Demolished()
        {
            // 给所在的地图的该位置撤销已存在建筑物
            if (Data.IsObstacle)
                MapManager.Instance.RemoveFromObstaclesList(MapName, _completePos);
        }

        /// <summary>
        /// 获取使用权限
        /// </summary>
        public virtual bool GetPrivilege(Pawn pawn)
        {
            if(isUsed) return false;

            isUsed = true;
            TheUsingPawn = pawn;
            return true;
        }

        /// <summary>
        /// 归还使用权限
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public virtual bool RevokePrivilege(Pawn pawn)
        {
            if(TheUsingPawn != pawn) return false;

            isUsed = false;
            TheUsingPawn = null;
            return true;
        }

        private void Update()
        {
            if(WorkLoad <= 0 && !isCompletely)
            {
                isCompletely = true;
                Complete();            
            }
        }
    }
}