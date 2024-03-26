using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using ChenChen_MapGenerator;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace ChenChen_BuildingSystem
{
    /// <summary>
    /// 打开建造模式，放置建筑什么的
    /// </summary>
    public class BuildingModeTool : MonoBehaviour
    {
        /// <summary>
        /// 所属BuildingSystemManager
        /// </summary>
        [SerializeField] private BuildingSystemManager buildingSystemManager;

        /// <summary>
        ///  当前地图的放建筑物的瓦片地图
        /// </summary>
        [SerializeField] private Tilemap BuildingTilemap;

        /// <summary>
        ///  当前地图的放墙体的瓦片地图
        /// </summary>
        [SerializeField] private Tilemap WallTilemap;

        /// <summary>
        /// 当前蓝图的名字
        /// </summary>
        [SerializeField] private string CurBuildingName;

        /// <summary>
        /// 当前的蓝图数据
        /// </summary>
        [SerializeField] private BlueprintData blueprintData;

        /// <summary>
        /// 当前鼠标上的蓝图预览
        /// </summary>
        [SerializeField] private GameObject MouseIndicator;

        /// <summary>
        /// 是否在建造模式下
        /// </summary>
        [SerializeField] private bool OnBuildMode;

        public BuildingModeTool()
        {
        }

        private void Start()
        {
            buildingSystemManager = GetComponent<BuildingSystemManager>();
        }

        private void Update()
        {
            BuildUpdate();
        }

        #region  Public

        /// <summary>
        /// 根据 name 切换蓝图
        /// </summary>
        /// <param name="name"></param>
        public void ToggleBlueprint(string name)
        {
            BuildStart(name);
            CurBuildingName = name;
        }

        #endregion


        private void BuildStart(string name)
        {
            //  找到当前应该放置的蓝图
            blueprintData = BuildingSystemManager.Instance.GetData(name);
            if (blueprintData == null)
            {
                Debug.LogWarning($"不存在蓝图: {name}, 已返回");
                OnBuildMode = false;
                return;
            }
            else
            {
                // 获取TileMap
                BuildingTilemap = MapManager.Instance.GetChildObject("Building").GetComponent<Tilemap>();
                WallTilemap = MapManager.Instance.GetChildObject("Wall").GetComponent<Tilemap>();

                // 配置鼠标指示器信息
                OnBuildMode = true;
                MouseIndicator = UnityEngine.Object.Instantiate(blueprintData.Prefab);
                if (!MouseIndicator.GetComponent<BoxCollider2D>())
                {
                    Debug.LogError("ERROR");
                    return;
                }
                MouseIndicator.SetActive(true);

            }
        }

        /// <summary>
        /// 建造的Update，监听鼠标信息，放置蓝图等
        /// </summary>
        private void BuildUpdate()
        {
            if (OnBuildMode && MouseIndicator != null)
            {
                // 监听鼠标位置信息转换成世界坐标
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPosition = BuildingTilemap.WorldToCell(mousePosition);
                Vector3 placePosition = BuildingTilemap.CellToWorld(cellPosition) + new Vector3(0.5f, 0.5f);
                MouseIndicator.transform.position = placePosition;

                SpriteRenderer sr = MouseIndicator.GetComponent<SpriteRenderer>();
                // 如果能建造则设置主体为绿色，否则为红色
                if (MapManager.Instance.CheckObjectWhetherCanPlaceOnHere(MouseIndicator))
                {
                    sr.color = Color.green;
                    // 放置
                    if (Input.GetMouseButtonDown(0))
                    {
                        GameObject newObject = UnityEngine.Object.Instantiate(blueprintData.Prefab,
                                                                              placePosition,
                                                                              MouseIndicator.transform.rotation,
                                                                              buildingSystemManager.transform);
                        BlueprintBase blueprint = newObject.GetComponent<BlueprintBase>();
                        blueprint.Placed();
                    }
                }
                else
                {
                    sr.color = Color.red;
                }
                // 当按下Q键，物体逆时针旋转90度
                if (Input.GetKeyDown(KeyCode.E))
                {
                    MouseIndicator.transform.Rotate(Vector3.forward, -90f);
                }
                // 当按下Q键，物体顺时针旋转90度
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    MouseIndicator.transform.Rotate(Vector3.forward, 90f);
                }
                //取消
                if (Input.GetMouseButtonDown(1))
                {
                    BuildEnd();
                }
            }
        }

        private void BuildEnd()
        {
            OnBuildMode = false;
            MouseIndicator.SetActive(false);
            UnityEngine.Object.Destroy(MouseIndicator);
        }
    }
}