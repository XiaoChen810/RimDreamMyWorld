using ChenChen_AI;
using ChenChen_MapGenerator;
using ChenChen_UISystem;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ChenChen_BuildingSystem
{
    public class Thing_Building : ThingBase
    {
        #region Life_Built

        public override void OnPlaced(BuildingLifeStateType initial_State, string mapName)
        {
            // ���ó�ʼֵ
            _workload = WorkloadBuilt;
            MapName = mapName;
            // ������ײ��
            if (TryGetComponent<Collider2D>(out Collider2D coll))
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
            // �жϳ�ʼ״̬���費��Ҫ��־����
            LifeState = initial_State;
            if(LifeState == BuildingLifeStateType.None)
            {
                LifeState = Workload <= 0 ? BuildingLifeStateType.FinishedBuilding : BuildingLifeStateType.MarkBuilding;
            }      
            // ������һ�к�
            BuildingSystemManager.Instance.AddThingToList(this.gameObject);
        }

        public override void OnMarkBuild()
        {
            // ��ɰ�͸������ʾ��δ���
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(1, 1, 1, 0f);
            DrawOutline_Collider = true;
        }

        public override void OnBuild(int value)
        {
            Workload -= value;
            if(Workload <= 0)
            {
                Workload = 0;
                LifeState = BuildingLifeStateType.FinishedBuilding;
            }
        }

        public override void OnComplete()
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();

            // ����Ƭ��ͼ������Ƭ
            if (Def.TileBase != null)
            {
                if (MapManager.Instance.TryGetTilemap("Building", true, out Tilemap buildingTilemap))
                {
                    buildingTilemap.SetTile(StaticFuction.VectorTransToInt(transform.position), Def.TileBase);
                    sr.color = new Color(1, 1, 1, 0f);
                }
                else
                {
                    Debug.LogError("Error in set tile");
                }
            }
            else
            {
                sr.color = new Color(1, 1, 1, 1f);
            }

            // ������ϰ���,�����ڵĵ�ͼ�ĸ�λ�����ô����ϰ���,������ײ��
            if (Def.IsObstacle)
            {
                GetComponent<Collider2D>().isTrigger = false;
                gameObject.layer = 8; //"Obstacle"
                Bounds bounds = ColliderSelf.bounds;
                AstarPath.active.UpdateGraphs(bounds);
            }
            IsDismantlable = true;
            DrawOutline_Collider = false;
        }

        public override void OnCancel()
        {
            BuildingSystemManager.Instance.RemoveThingToList(this.gameObject);
            Destroy(gameObject);
        }

        public override void OnInterpret()
        {
            LifeState = BuildingLifeStateType.MarkBuilding;
        }

        #endregion

        #region Life_Demolish

        public override void OnMarkDemolish()
        {
            _workload = Mathf.CeilToInt(Def.Workload * 0.5f);
        }

        public override void OnDemolish(int value)
        {
            _workload -= value;
            if(_workload <= 0)
            {
                _workload = 0;
                LifeState = BuildingLifeStateType.FinishedDemolished;
            }
        }

        public override void OnDemolished()
        {
            if (Def.TileBase != null)
            {
                if (MapManager.Instance.TryGetTilemap("Building", true, out Tilemap buildingTilemap))
                {
                    buildingTilemap.SetTile(StaticFuction.VectorTransToInt(transform.position), null);
                }
            }
            if (_detailView.onShow)
            {
                DetailViewPanel detail = PanelManager.Instance.GetTopPanel() as DetailViewPanel;
                PanelManager.Instance.RemovePanel(detail);
            }
            BuildingSystemManager.Instance.RemoveThingToList(this.gameObject);
            FindAnyObjectByType<AstarPath>().Scan();
            Debug.Log($"�Ƴ�������" + gameObject.name);
            Destroy(gameObject);
        }

        #endregion

        #region Privilege

        /// <summary>
        /// ��ȡʹ��Ȩ��
        /// </summary>
        public virtual bool GetPrivilege(Pawn pawn)
        {
            if(_isUsed) return false;

            _isUsed = true;
            _theUsingPawn = pawn;
            return true;
        }

        /// <summary>
        /// �黹ʹ��Ȩ��
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public virtual bool RevokePrivilege(Pawn pawn)
        {
            if(_theUsingPawn != pawn) return false;

            _isUsed = false;
            _theUsingPawn = null;
            return true;
        }

        #endregion

        protected override void OnEnable()
        {
            base.OnEnable();
        }
    }
}