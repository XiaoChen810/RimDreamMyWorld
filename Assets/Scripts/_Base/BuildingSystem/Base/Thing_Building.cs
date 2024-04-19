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

        [SerializeField] private BuildingStateType _buildingState;
        public BuildingStateType State
        {
            get
            {
                return _buildingState;
            }
        }


        #region Life_Built

        public override void Placed()
        {
            // 设置初始值
            NeedWorkload = WorkloadBuilt;
            _buildingState = NeedWorkload <= 0 ? BuildingStateType.FinishedBuilding : BuildingStateType.WaitingBuilt;
            _buildingTilemap = BuildingSystemManager.Instance.Tool.BuildingTilemap;
            _completePos = _buildingTilemap.WorldToCell(transform.position);

            // 变成半透明，表示还未完成
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(1, 1, 1, 0.5f);

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

            // 设置完一切后
            BuildingSystemManager.Instance.AddBuildingToList(this.gameObject);
        }

        public override void Build(int thisWorkload)
        {
            NeedWorkload -= thisWorkload;
        }

        public override void Complete()
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();

            // 在瓦片地图设置瓦片
            if (Data.TileBase != null)
            {
                _buildingTilemap.SetTile(_completePos, Data.TileBase);
                sr.color = new Color(1, 1, 1, 0f);
            }
            else
            {
                sr.color = new Color(1, 1, 1, 1f);
            }

            // 如果是障碍物,给所在的地图的该位置设置存在障碍物
            if (Data.IsObstacle) MapManager.Instance.AddToObstaclesList(this.gameObject);
            // 如果是障碍物，则设置碰撞体
            if (Data.IsObstacle) GetComponent<Collider2D>().isTrigger = false;
            FindAnyObjectByType<AstarPath>().Scan();
            IsDismantlable = true;
            _buildingState = BuildingStateType.FinishedBuilding;
        }

        public override void Cancel()
        {
            BuildingSystemManager.Instance.RemoveBuildingToList(this.gameObject);
            Destroy(gameObject);
        }

        public override void Interpret()
        {
            _buildingState = BuildingStateType.WaitingBuilt;
        }

        #endregion

        #region Life_Demolish

        public override void OnMarkDemolish()
        {
            _buildingState = BuildingStateType.WaitingDemolished;
            _needWorkload = Mathf.CeilToInt(Data.Workload * 0.5f);
        }

        public override void Demolish(int value)
        {
            _needWorkload -= value;
        }

        public override void OnDemolished()
        {
            if (Data.IsObstacle) MapManager.Instance.RemoveFromObstaclesList(this.gameObject);
            if (Data.TileBase != null) _buildingTilemap.SetTile(_completePos, null);
            if(_detailView.onShow)
            {
                DetailViewPanel detail = PanelManager.Instance.GetTopPanel() as DetailViewPanel;
                PanelManager.Instance.RemovePanel(detail);
            }
            BuildingSystemManager.Instance.RemoveBuildingToList(this.gameObject);
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
            _detailView = gameObject.AddComponent<DetailView_Building>();
        }         

        private void Update()
        {
            if(_buildingState == BuildingStateType.WaitingBuilt && NeedWorkload <= 0)
            {
                Complete();
                _buildingState = BuildingStateType.FinishedBuilding;
                return;
            }

            if(_buildingState == BuildingStateType.WaitingDemolished && NeedWorkload <= 0)
            {
                OnDemolished();
                _buildingState = BuildingStateType.None;
                return;
            }
        }


    }
}