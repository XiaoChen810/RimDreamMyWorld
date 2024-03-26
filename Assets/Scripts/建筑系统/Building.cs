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
        /// �ж��Ƿ��Ѿ���������ʹ��
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

            // ��ɰ�͸������ʾ��δ���
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(1, 1, 1, 0.5f);

            // ��ӵ���������ϵͳ��
            if(WorkLoad <= 0)
            {
                BuildingSystemManager.Instance.AddBuildingListDone(this.gameObject);
            }
            else
            {
                BuildingSystemManager.Instance.AddTask(this.gameObject);
            }

            // ������ײ��
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

            // ����Ƭ��ͼ������Ƭ
            if (Data.TileBase != null)
            {
                _wallTilemap.SetTile(_completePos, Data.TileBase);
                sr.color = new Color(1, 1, 1, 0f);
            }
            else
            {
                sr.color = new Color(1, 1, 1, 1f);
            }

            // ������ϰ����������ײ��,�����ڵĵ�ͼ�ĸ�λ�����ô����ϰ���
            if (Data.IsObstacle)
            {
                MapManager.Instance.AddToObstaclesList(MapName, _completePos);
                if (Data.IsObstacle) GetComponent<Collider2D>().isTrigger = false;
            }

            // ��BuildingSystemManager����ɽ���
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
            // �����ڵĵ�ͼ�ĸ�λ�ó����Ѵ��ڽ�����
            if (Data.IsObstacle)
                MapManager.Instance.RemoveFromObstaclesList(MapName, _completePos);
        }

        /// <summary>
        /// ��ȡʹ��Ȩ��
        /// </summary>
        public virtual bool GetPrivilege(Pawn pawn)
        {
            if(isUsed) return false;

            isUsed = true;
            TheUsingPawn = pawn;
            return true;
        }

        /// <summary>
        /// �黹ʹ��Ȩ��
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