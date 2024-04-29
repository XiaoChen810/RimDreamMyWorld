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

        [SerializeField] private BuildingLifeStateType _buildingState;
        public BuildingLifeStateType State
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
            _buildingState = NeedWorkload <= 0 ? BuildingLifeStateType.FinishedBuilding : BuildingLifeStateType.WaitingBuilt;
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
            BuildingSystemManager.Instance.AddThingToList(this.gameObject);
        }

        public override void Build(int thisWorkload)
        {
            NeedWorkload -= thisWorkload;
        }

        public override void Complete()
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
            FindAnyObjectByType<AstarPath>().Scan();
            IsDismantlable = true;
            _buildingState = BuildingLifeStateType.FinishedBuilding;
        }

        public override void Cancel()
        {
            BuildingSystemManager.Instance.RemoveThingToList(this.gameObject);
            Destroy(gameObject);
        }

        public override void Interpret()
        {
            _buildingState = BuildingLifeStateType.WaitingBuilt;
        }

        #endregion

        #region Life_Demolish

        public override void OnMarkDemolish()
        {
            _buildingState = BuildingLifeStateType.WaitingDemolished;
            _needWorkload = Mathf.CeilToInt(Def.Workload * 0.5f);
        }

        public override void Demolish(int value)
        {
            _needWorkload -= value;
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
            _detailView = gameObject.AddComponent<DetailView_Building>();
            _buildingTilemap = MapManager.Instance.GetTilemap("Building");
        }         

        private void Update()
        {
            if(_buildingState == BuildingLifeStateType.WaitingBuilt && NeedWorkload <= 0)
            {
                Complete();
                _buildingState = BuildingLifeStateType.FinishedBuilding;
                return;
            }

            if(_buildingState == BuildingLifeStateType.WaitingDemolished && NeedWorkload <= 0)
            {
                OnDemolished();
                _buildingState = BuildingLifeStateType.None;
                return;
            }
        }


    }
}