using ChenChen_AI;
using ChenChen_Map;
using ChenChen_UI;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ChenChen_Thing
{
    public class Thing_Building : ThingBase
    {
        #region Life_Built

        public override void OnPlaced(BuildingLifeStateType initial_State, string mapName)
        {
            // ���ó�ʼֵ
            Workload = WorkloadBuilt;
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
            ChangeLifeState(initial_State);
            if(_lifeState == BuildingLifeStateType.None)
            {
                ChangeLifeState(Workload <= 0 ? BuildingLifeStateType.FinishedBuilding : BuildingLifeStateType.MarkBuilding);
            }      
        }

        public override void OnMarkBuild()
        {
            // ��ɰ�͸������ʾ��δ���
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(1, 1, 1, 0f);
            DrawOutline_Collider = true;
            Workload = Workload > 0 ? Workload :WorkloadBuilt;
        }

        public override void OnBuild(int value)
        {
            Workload -= value;
            if(Workload <= 0)
            {
                Workload = 0;
                ChangeLifeState(BuildingLifeStateType.FinishedBuilding);
            }
        }

        public override void OnCompleteBuild()
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
                if (AstarPath.active != null)
                {
                    Bounds bounds = ColliderSelf.bounds;
                    AstarPath.active.UpdateGraphs(bounds);
                }
            }
            CanDemolish = true;
            DrawOutline_Collider = false;
            Workload = 0;
        }

        public override void OnCancelBuild()
        {
            Destroy(gameObject);
        }

        public override void OnInterpretBuild()
        {
            ChangeLifeState(BuildingLifeStateType.MarkBuilding);
        }

        #endregion

        #region Life_Demolish

        public override void OnMarkDemolish()
        {
            _workload = Mathf.CeilToInt(Def.Workload * 0.5f);
            if(Workload == 0)
            {
                ChangeLifeState(BuildingLifeStateType.FinishedDemolished);
            }
        }

        public override void OnDemolish(int value)
        {
            _workload -= value;
            if(_workload <= 0)
            {
                _workload = 0;
                ChangeLifeState(BuildingLifeStateType.FinishedDemolished);
            }
        }

        public override void OnDemolished()
        {
            Debug.Log($"�Ƴ�������" + gameObject.name);
            Destroy(gameObject);
        }

        public override void OnCanclDemolish()
        {
            ChangeLifeState(BuildingLifeStateType.FinishedBuilding);
        }

        #endregion

        protected override void OnEnable()
        {
            base.OnEnable();
        }
    }
}