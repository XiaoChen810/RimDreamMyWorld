using ChenChen_Map;
using UnityEngine.Tilemaps;
using UnityEngine;

namespace ChenChen_Thing
{
    public class Thing_Architectural : ThingBase
    {
        #region Life_Built

        public override void OnPlaced(BuildingLifeStateType initial_State, string mapName)
        {
            // 设置初始值
            Workload = WorkloadBuilt;
            CurDurability = MaxDurability;
            MapName = mapName;

            // 判断初始状态，需不需要标记建造
            ChangeLifeState(initial_State);
            if (_lifeState == BuildingLifeStateType.None)
            {
                ChangeLifeState(Workload <= 0 ? BuildingLifeStateType.FinishedBuilding : BuildingLifeStateType.MarkBuilding);
            }
        }

        public override void OnMarkBuild()
        {       
            SR.color = new Color(1, 1, 1, 0f);
            DrawOutline_Collider = true;
            Workload = Workload > 0 ? Workload : WorkloadBuilt;
        }

        public override void OnBuild(int value)
        {
            Workload -= value;
            if (Workload <= 0)
            {
                Workload = 0;
                ChangeLifeState(BuildingLifeStateType.FinishedBuilding);
            }
        }

        public override void OnCompleteBuild()
        {
            // 在瓦片地图设置瓦片
            if (Def.TileBase != null)
            {
                if (MapManager.Instance.TryGetTilemap(_tilemapName, true, out Tilemap tilemap))
                {
                    tilemap.SetTile(StaticFuction.VectorTransToInt(transform.position), Def.TileBase);
                    SR.color = new Color(1, 1, 1, 0f);
                }
                else
                {
                    Debug.LogError("Error in set tile");
                }
            }
            else
            {
                SR.color = new Color(1, 1, 1, 1f);
            }

            // 如果是障碍物,给所在的地图的该位置设置存在障碍物,设置碰撞体
            if (Def.IsObstacle)
            {
                ColliderSelf.isTrigger = false;
                gameObject.layer = 8; //"Obstacle"
                if (AstarPath.active != null)
                {
                    Bounds bounds = ColliderSelf.bounds;
                    AstarPath.active.UpdateGraphs(bounds);
                }
            }
            CanDemolish = true;
            DrawOutline_Collider = false;
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
            if (Workload == 0)
            {
                ChangeLifeState(BuildingLifeStateType.FinishedDemolished);
            }
        }

        public override void OnDemolish(int value)
        {
            _workload -= value;
            if (_workload <= 0)
            {
                _workload = 0;
                ChangeLifeState(BuildingLifeStateType.FinishedDemolished);
            }
        }

        public override void OnDemolished()
        {
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