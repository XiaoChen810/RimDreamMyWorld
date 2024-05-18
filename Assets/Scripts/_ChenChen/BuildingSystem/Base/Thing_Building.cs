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
            // 设置初始值
            Workload = WorkloadBuilt;
            MapName = mapName;
            // 设置碰撞体
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
            // 判断初始状态，需不需要标志建造
            ChangeLifeState(initial_State);
            if(_lifeState == BuildingLifeStateType.None)
            {
                ChangeLifeState(Workload <= 0 ? BuildingLifeStateType.FinishedBuilding : BuildingLifeStateType.MarkBuilding);
            }      
            // 设置完一切后
            ThingSystemManager.Instance.AddThingToList(this.gameObject);
        }

        public override void OnMarkBuild()
        {
            // 变成半透明，表示还未完成
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

        public override void OnComplete()
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();

            // 在瓦片地图设置瓦片
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

            // 如果是障碍物,给所在的地图的该位置设置存在障碍物,设置碰撞体
            if (Def.IsObstacle)
            {
                GetComponent<Collider2D>().isTrigger = false;
                gameObject.layer = 8; //"Obstacle"
                Bounds bounds = ColliderSelf.bounds;
                AstarPath.active.UpdateGraphs(bounds);
            }
            CanDemolish = true;
            DrawOutline_Collider = false;
            Workload = 0;
        }

        public override void OnCancel()
        {
            ThingSystemManager.Instance.RemoveThingToList(this.gameObject);
            Destroy(gameObject);
        }

        public override void OnInterpret()
        {
            ChangeLifeState(BuildingLifeStateType.MarkBuilding);
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
                ChangeLifeState(BuildingLifeStateType.FinishedDemolished);
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
            if (_detailView.OnShow)
            {
                PanelManager panel = DetailViewManager.Instance.PanelManager;
                panel.RemoveTopPanel(panel.GetTopPanel());
            }
            if (Def.IsObstacle)
            {
                Bounds bounds = ColliderSelf.bounds;
                AstarPath.active.UpdateGraphs(bounds);
            }
            ThingSystemManager.Instance.RemoveThingToList(this.gameObject);
            Debug.Log($"移除建筑：" + gameObject.name);
            Destroy(gameObject);
        }

        #endregion

        protected override void OnEnable()
        {
            base.OnEnable();
        }
    }
}