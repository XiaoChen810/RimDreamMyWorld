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
            // 设置初始值
            _workload = WorkloadBuilt;
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
            LifeState = initial_State;
            if(LifeState == BuildingLifeStateType.None)
            {
                LifeState = Workload <= 0 ? BuildingLifeStateType.FinishedBuilding : BuildingLifeStateType.MarkBuilding;
            }      
            // 设置完一切后
            BuildingSystemManager.Instance.AddThingToList(this.gameObject);
        }
        public override void OnMarkBuild()
        {
            // 变成半透明，表示还未完成
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

            // 在瓦片地图设置瓦片
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

            // 如果是障碍物,给所在的地图的该位置设置存在障碍物,设置碰撞体
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


        private Color centerColor = new Color(0f, 0f, 1f, 0f); // 透明蓝色
        private Color outlineColor = Color.white; // 纯白色
        private float outlineWidth = 2f;

        private Texture2D centerTexture;
        private Texture2D outlineTexture;

        protected override void OnEnable()
        {
            base.OnEnable();
            // 创建中心透明蓝色纹理
            centerTexture = new Texture2D(1, 1);
            centerTexture.SetPixel(0, 0, centerColor);
            centerTexture.Apply();

            // 创建纯白色纹理用于绘制边框
            outlineTexture = new Texture2D(1, 1);
            outlineTexture.SetPixel(0, 0, outlineColor);
            outlineTexture.Apply();
        }

        private void OnGUI()
        {
            if (_drawOutline)
            {
                // 计算建造物体的边界框
                Bounds bounds = GetComponent<Collider2D>().bounds;

                // 计算建造物体边界框在屏幕上的位置和大小
                Vector3 screenBoundsMin = Camera.main.WorldToScreenPoint(bounds.min);
                Vector3 screenBoundsMax = Camera.main.WorldToScreenPoint(bounds.max);
                Vector3 screenSize = screenBoundsMax - screenBoundsMin;

                // 绘制中心透明蓝色矩形
                GUI.DrawTexture(new Rect(screenBoundsMin.x, Screen.height - screenBoundsMax.y, screenSize.x, screenSize.y), centerTexture);

                // 绘制边框
                GUI.DrawTexture(new Rect(screenBoundsMin.x, Screen.height - screenBoundsMax.y, screenSize.x, outlineWidth), outlineTexture); // 上边框
                GUI.DrawTexture(new Rect(screenBoundsMin.x, Screen.height - screenBoundsMax.y, outlineWidth, screenSize.y), outlineTexture); // 左边框
                GUI.DrawTexture(new Rect(screenBoundsMax.x - outlineWidth, Screen.height - screenBoundsMax.y, outlineWidth, screenSize.y), outlineTexture); // 右边框
                GUI.DrawTexture(new Rect(screenBoundsMin.x, Screen.height - screenBoundsMin.y, screenSize.x, outlineWidth), outlineTexture); // 下边框
            }
        }
    }
}