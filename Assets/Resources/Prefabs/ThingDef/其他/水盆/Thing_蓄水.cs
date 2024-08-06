using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChenChen_Map;
using ChenChen_UI;

namespace ChenChen_Thing
{
    public class Thing_蓄水 : Building
    {
        private LineRenderer lineRenderer;
        private GameManager gameManager;

        [Header("Water")]
        public float water = 0;
        [SerializeField] private float max_water_capital = 10;
        [SerializeField] private float impounding_speed = 1;
        [SerializeField] private float line_width = 0.1f;
        [SerializeField] private int line_max_deep = 7;
        [SerializeField] private Sprite full_icon;

        protected override void Start()
        {
            base.Start();

            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startWidth = line_width;
            lineRenderer.endWidth = line_width;
            lineRenderer.startColor = Color.blue;
            lineRenderer.endColor = Color.blue;
            lineRenderer.sortingLayerName = "Above";

            gameManager = GameManager.Instance;

            DetailView.OverrideContentAction = (DetailViewOverrideContentAction());
        }

        private Action<DetailViewPanel> DetailViewOverrideContentAction()
        {
            return (DetailViewPanel panel) =>
            {
                List<String> content = new List<String>();
                content.Add($"耐久度: {this.Durability} / {this.MaxDurability}");
                content.Add($"使用者: {(this.UserPawn != null ? this.UserPawn.name : null)}");
                if (this.Workload > 0)
                {
                    content.Add($"剩余工作量: {this.Workload}");
                }
                if (this.LifeState == BuildingLifeStateType.FinishedBuilding)
                {
                    content.Add($"蓄水量: {this.water.ToString("0.0")}");
                }
                panel.SetView(this.Def.DefName, content);
                if (this.LifeState == BuildingLifeStateType.MarkBuilding)
                {
                    panel.RemoveAllButton();
                    panel.SetButton("取消", () =>
                    {
                        this.OnCancelBuild();
                    });
                }
                if (this.LifeState == BuildingLifeStateType.MarkDemolished)
                {
                    panel.RemoveAllButton();
                    panel.SetButton("取消", () =>
                    {
                        this.OnCanclDemolish();
                    });
                }
                if (this.LifeState == BuildingLifeStateType.FinishedBuilding)
                {
                    panel.RemoveAllButton();
                    panel.SetButton("拆除", () =>
                    {
                        this.MarkToDemolish();
                    });
                }
            };
        }

        private void Update()
        {
            if (gameManager.Mode_Water)
            {
                var nearestWaterNode = FindNearestWater();
                if (nearestWaterNode != null)
                {
                    Vector2 water = new Vector3(nearestWaterNode.position.x, nearestWaterNode.position.y) + new Vector3 (0.5f, 0.5f, 0);
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPosition(0, transform.position);
                    lineRenderer.SetPosition(1, water);
                }
                else
                {
                    lineRenderer.positionCount = 0;
                }
            }
            else
            {
                lineRenderer.positionCount = 0;
            }

            if (LifeState == BuildingLifeStateType.FinishedBuilding)
            {
                water += Time.deltaTime * impounding_speed;

                if (water >= max_water_capital)
                {
                    water = max_water_capital;
                    SR.sprite = full_icon;
                }
            }
        }

        #region - Water -
        private MapNode FindNearestWater()
        {
            MapManager mapManager = MapManager.Instance;
            var startNode = mapManager.GetMapNodeHere(transform.position);

            Queue<MapNode> stack = new Queue<MapNode>();
            HashSet<MapNode> visited = new HashSet<MapNode>();

            stack.Enqueue(startNode);
            visited.Add(startNode);

            Vector2Int[] directions = new Vector2Int[]
            {
                new Vector2Int(0, 1),  // 上
                new Vector2Int(0, -1), // 下
                new Vector2Int(1, 0),  // 右
                new Vector2Int(-1, 0), // 左
                new Vector2Int(1, 1),  // 右上
                new Vector2Int(1, -1), // 右下
                new Vector2Int(-1, 1), // 左上
                new Vector2Int(-1, -1) // 左下
            };

            while (stack.Count > 0)
            {
                var curNode = stack.Dequeue();
                Vector2Int curPos = curNode.position;

                if (curNode.type == NodeType.water && CheckNeiborNodeIsWater(curPos))
                {
                    return curNode;
                }

                // 优先处理上下左右方向
                for (int i = 0; i < 4; i++)
                {
                    var neighborPos = curPos + directions[i];
                    var neighbor = mapManager.GetMapNodeHere(neighborPos);
                    if (neighbor != null && !visited.Contains(neighbor) && Vector2.Distance(neighbor.position, transform.position) < line_max_deep)
                    {
                        stack.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                }

                // 然后处理对角线方向
                for (int i = 4; i < directions.Length; i++)
                {
                    var neighborPos = curPos + directions[i];
                    var neighbor = mapManager.GetMapNodeHere(neighborPos);
                    if (neighbor != null && !visited.Contains(neighbor) && Vector2.Distance(neighbor.position, transform.position) < line_max_deep)
                    {
                        stack.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }

            return null;
        }

        private bool CheckNeiborNodeIsWater(Vector2Int curPos)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    var neighborPos = curPos + new Vector2Int(x, y);
                    var neighbor = MapManager.Instance.GetMapNodeHere(neighborPos);
                    if (neighbor != null && neighbor.type != NodeType.water)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion
    }
}