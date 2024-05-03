using ChenChen_AI;
using ChenChen_MapGenerator;
using ChenChen_UISystem;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Networking.UnityWebRequest;

namespace ChenChen_BuildingSystem
{
    public class Thing_Building : ThingBase
    {
        private bool _drawOutline;

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
            _drawOutline = true;
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
                if (MapManager.Instance.TryGetTilemap("Building", out Tilemap buildingTilemap))
                {
                    buildingTilemap.SetTile(StaticFuction.VectorTransToInt(transform.position), Def.TileBase);
                    sr.color = new Color(1, 1, 1, 0f);
                    Debug.Log("Compele");
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
            }
            IsDismantlable = true;
            _drawOutline = false;
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
                if (MapManager.Instance.TryGetTilemap("Building", out Tilemap buildingTilemap))
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


        private Color centerColor = new Color(0f, 0f, 1f, 0f); // ͸����ɫ
        private Color outlineColor = Color.white; // ����ɫ
        private float outlineWidth = 2f;

        private Texture2D centerTexture;
        private Texture2D outlineTexture;

        protected override void OnEnable()
        {
            base.OnEnable();
            // ��������͸����ɫ����
            centerTexture = new Texture2D(1, 1);
            centerTexture.SetPixel(0, 0, centerColor);
            centerTexture.Apply();

            // ��������ɫ�������ڻ��Ʊ߿�
            outlineTexture = new Texture2D(1, 1);
            outlineTexture.SetPixel(0, 0, outlineColor);
            outlineTexture.Apply();
        }

        private void OnGUI()
        {
            if (_drawOutline)
            {
                // ���㽨������ı߽��
                Bounds bounds = GetComponent<Collider2D>().bounds;

                // ���㽨������߽������Ļ�ϵ�λ�úʹ�С
                Vector3 screenBoundsMin = Camera.main.WorldToScreenPoint(bounds.min);
                Vector3 screenBoundsMax = Camera.main.WorldToScreenPoint(bounds.max);
                Vector3 screenSize = screenBoundsMax - screenBoundsMin;

                // ��������͸����ɫ����
                GUI.DrawTexture(new Rect(screenBoundsMin.x, Screen.height - screenBoundsMax.y, screenSize.x, screenSize.y), centerTexture);

                // ���Ʊ߿�
                GUI.DrawTexture(new Rect(screenBoundsMin.x, Screen.height - screenBoundsMax.y, screenSize.x, outlineWidth), outlineTexture); // �ϱ߿�
                GUI.DrawTexture(new Rect(screenBoundsMin.x, Screen.height - screenBoundsMax.y, outlineWidth, screenSize.y), outlineTexture); // ��߿�
                GUI.DrawTexture(new Rect(screenBoundsMax.x - outlineWidth, Screen.height - screenBoundsMax.y, outlineWidth, screenSize.y), outlineTexture); // �ұ߿�
                GUI.DrawTexture(new Rect(screenBoundsMin.x, Screen.height - screenBoundsMin.y, screenSize.x, outlineWidth), outlineTexture); // �±߿�
            }
        }
    }
}