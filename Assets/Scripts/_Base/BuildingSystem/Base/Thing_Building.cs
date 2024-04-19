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
            // ���ó�ʼֵ
            NeedWorkload = WorkloadBuilt;
            _buildingState = NeedWorkload <= 0 ? BuildingStateType.FinishedBuilding : BuildingStateType.WaitingBuilt;
            _buildingTilemap = BuildingSystemManager.Instance.Tool.BuildingTilemap;
            _completePos = _buildingTilemap.WorldToCell(transform.position);

            // ��ɰ�͸������ʾ��δ���
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(1, 1, 1, 0.5f);

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

            // ������һ�к�
            BuildingSystemManager.Instance.AddBuildingToList(this.gameObject);
        }

        public override void Build(int thisWorkload)
        {
            NeedWorkload -= thisWorkload;
        }

        public override void Complete()
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();

            // ����Ƭ��ͼ������Ƭ
            if (Data.TileBase != null)
            {
                _buildingTilemap.SetTile(_completePos, Data.TileBase);
                sr.color = new Color(1, 1, 1, 0f);
            }
            else
            {
                sr.color = new Color(1, 1, 1, 1f);
            }

            // ������ϰ���,�����ڵĵ�ͼ�ĸ�λ�����ô����ϰ���
            if (Data.IsObstacle) MapManager.Instance.AddToObstaclesList(this.gameObject);
            // ������ϰ����������ײ��
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