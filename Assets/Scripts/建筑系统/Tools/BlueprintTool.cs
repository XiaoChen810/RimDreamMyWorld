using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using MyMapGenerate;

namespace MyBuildingSystem
{
    /// <summary>
    /// 打开建造模式，放置建筑蓝图什么的
    /// </summary>
    public class BlueprintTool
    {
        /// <summary>
        /// 所属的建造系统管理者
        /// </summary>
        public BuildingSystemManager BSM { get; private set; }

        /// <summary>
        ///  当前地图的放建筑物的瓦片地图
        /// </summary>
        public Tilemap BuildingTilemap { get; private set; }

        /// <summary>
        ///  当前地图的放墙体的瓦片地图
        /// </summary>
        public Tilemap WallTilemap { get; private set; }

        /// <summary>
        /// 当条件触发时调用的事件
        /// </summary>
        public Action<string> OnToggle;

        /// <summary>
        /// 当条件触发时调用的事件
        /// </summary>
        public Action<Vector3> OnPlace, OnCancel;

        /// <summary>
        /// 当前蓝图的名字
        /// </summary>
        public string BlueprintName { get; private set; }

        /// <summary>
        /// 当前的蓝图数据
        /// </summary>
        public BlueprintData blueprintData { get; private set; }

        /// <summary>
        /// 当前鼠标上的蓝图预览
        /// </summary>
        public GameObject MouseIndicator { get; private set; }

        /// <summary>
        /// 是否在建造模式下
        /// </summary>
        public bool OnBuildMode { get; private set; }

        public BlueprintTool(BuildingSystemManager buildingSystemManager)
        {
            this.BSM = buildingSystemManager;
            this.MouseIndicator = BSM.MouseIndicator;
        }

        #region  Public

        /// <summary>
        /// 根据 name 切换蓝图
        /// </summary>
        /// <param name="name"></param>
        public void ToggleBlueprint(string name)
        {
            OnToggle?.Invoke(name);
            BuildStart(name);
        }

        /// <summary>
        /// 建造的Update，监听鼠标信息，放置蓝图等
        /// </summary>
        public void BuildUpdate()
        {
            if (OnBuildMode)
            {
                // 监听鼠标位置信息转换成世界坐标
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPosition = BuildingTilemap.WorldToCell(mousePosition);
                Vector3 worldPosition = BuildingTilemap.CellToWorld(cellPosition);
                Vector3 placePosition = worldPosition + new Vector3(0.5f, 0.5f);
                MouseIndicator.transform.position = cellPosition + new Vector3(0.5f, 0.5f);

                SpriteRenderer sr = MouseIndicator.GetComponent<SpriteRenderer>();
                // 如果能建造则设置主体为绿色，否则为红色
                if (MapManager.Instance.CheckBlueprintCanPlaced(blueprintData, worldPosition))
                {
                    sr.color = Color.green;
                    // 放置
                    if (Input.GetMouseButtonDown(0))
                    {
                        OnPlace?.Invoke(cellPosition);
                        GameObject newObject = UnityEngine.Object.Instantiate(blueprintData.Prefab, placePosition, Quaternion.identity, BSM.transform);
                        BlueprintBase blueprint = newObject.GetComponent<BlueprintBase>();
                        blueprint.Placed();
                    }
                }
                else
                {
                    sr.color = Color.red;
                }

                //取消
                if (Input.GetMouseButtonDown(1))
                {
                    OnCancel?.Invoke(cellPosition);
                    BuildEnd();
                }
            }
        }

        #endregion


        private void BuildStart(string name)
        {
            //  找到当前应该放置的蓝图
            blueprintData = BuildingSystemManager.Instance.GetData(name);
            if (blueprintData == null)
            {
                Debug.LogWarning("不存在蓝图, 已返回");
                BuildEnd();
                return;
            }
            else
            {
                // 获取TileMap
                BuildingTilemap = MapManager.Instance.GetChildObject("Building").GetComponent<Tilemap>();
                WallTilemap = MapManager.Instance.GetChildObject("Wall").GetComponent<Tilemap>();

                // 配置鼠标指示器信息
                OnBuildMode = true;
                MouseIndicator.SetActive(true);
                SpriteRenderer sr = MouseIndicator.GetComponent<SpriteRenderer>();
                sr.sprite = blueprintData.PreviewSprite;

            }
        }

        private void BuildEnd()
        {
            OnBuildMode = false;
            MouseIndicator.SetActive(false);
        }
    }
}