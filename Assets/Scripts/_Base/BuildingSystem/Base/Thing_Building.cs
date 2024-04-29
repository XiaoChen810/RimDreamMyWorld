using ChenChen_MapGenerator;
using ChenChen_UISystem;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ChenChen_BuildingSystem
{
    public class Thing_Building : ThingBase, IDetailView
    {
        private Tilemap _buildingTilemap;
        private Vector3Int _completePos;

        [SerializeField] protected Pawn _theUsingPawn;
        public Pawn TheUsingPawn
        {
            get
            {
                return _theUsingPawn;
            }
            protected set
            {
                _theUsingPawn = value;
            }
        }

        [SerializeField] protected bool _isUsed;
        public bool IsUsed
        {
            get
            {
                return _isUsed;
            }
            protected set
            {
                _isUsed = value;
            }
        }

        [SerializeField] private DetailView _detailView;
        public DetailView DetailView
        {
            get
            {
                if( _detailView == null)
                {
                    if(!TryGetComponent<DetailView>(out _detailView))
                    {
                        _detailView = gameObject.AddComponent<DetailView_Building>();
                    }
                }
                return _detailView;
            }
        }

        [SerializeField] private BuildingLifeStateType _state;
        public BuildingLifeStateType State
        {
            get
            {
                return _state;
            }
            set
            {
                if(value != _state )
                {
                    switch (value)
                    {
                        case BuildingLifeStateType.MarkBuilding:
                            OnMarkBuild();
                            break;
                        case BuildingLifeStateType.FinishedBuilding:
                            OnComplete();
                            break;
                        case BuildingLifeStateType.MarkDemolished:
                            OnMarkDemolish();
                            break;
                        case BuildingLifeStateType.FinishedDemolished:
                            OnDemolished();
                            break;

                    }
                    _state = value;
                }
            }
        }


        #region Life_Built

        public override void OnPlaced(BuildingLifeStateType initial_State = BuildingLifeStateType.None)
        {
            // 设置初始值
            _workload = WorkloadBuilt;
            _buildingTilemap = MapManager.Instance.GetTilemap("Building");
            _completePos = _buildingTilemap.WorldToCell(transform.position);
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
            State = initial_State;
            if(State == BuildingLifeStateType.None)
            {
                State = Workload <= 0 ? BuildingLifeStateType.FinishedBuilding : BuildingLifeStateType.MarkBuilding;
            }      
            // 设置完一切后
            BuildingSystemManager.Instance.AddThingToList(this.gameObject);
        }
        public override void OnMarkBuild()
        {
            // 变成半透明，表示还未完成
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(1, 1, 1, 0.3f);
        }

        public override void OnBuild(int value)
        {
            Workload -= value;
            if(Workload <= 0)
            {
                Workload = 0;
                State = BuildingLifeStateType.FinishedBuilding;
            }
        }

        public override void OnComplete()
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();

            // 在瓦片地图设置瓦片
            if (Def.TileBase != null)
            {
                _buildingTilemap.SetTile(_completePos, Def.TileBase);
                sr.color = new Color(1, 1, 1, 0f);
            }
            else
            {
                sr.color = new Color(1, 1, 1, 1f);
            }

            // 如果是障碍物,给所在的地图的该位置设置存在障碍物,设置碰撞体
            if (Def.IsObstacle)
            {
                MapManager.Instance.AddToObstaclesList(this.gameObject);
                GetComponent<Collider2D>().isTrigger = false;
                gameObject.layer = 8; //"Obstacle"
            }
            IsDismantlable = true;
        }

        public override void OnCancel()
        {
            BuildingSystemManager.Instance.RemoveThingToList(this.gameObject);
            Destroy(gameObject);
        }

        public override void OnInterpret()
        {
            State = BuildingLifeStateType.MarkBuilding;
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
                State = BuildingLifeStateType.FinishedDemolished;
            }
        }

        public override void OnDemolished()
        {
            if (Def.IsObstacle) MapManager.Instance.RemoveFromObstaclesList(this.gameObject);
            if (Def.TileBase != null) _buildingTilemap.SetTile(_completePos, null);
            if(_detailView.onShow)
            {
                DetailViewPanel detail = PanelManager.Instance.GetTopPanel() as DetailViewPanel;
                PanelManager.Instance.RemovePanel(detail);
            }
            BuildingSystemManager.Instance.RemoveThingToList(this.gameObject);
            FindAnyObjectByType<AstarPath>().Scan();
            Debug.Log($"移除建筑：" + gameObject.name);
            Destroy(gameObject);
        }

        #endregion

        #region Privilege

        /// <summary>
        /// 获取使用权限
        /// </summary>
        public virtual bool GetPrivilege(Pawn pawn)
        {
            if(_isUsed) return false;

            _isUsed = true;
            _theUsingPawn = pawn;
            return true;
        }

        /// <summary>
        /// 归还使用权限
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