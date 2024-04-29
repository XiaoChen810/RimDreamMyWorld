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
    public class BuildingModeTool
    {
        /// <summary>
        /// 当前蓝图的名字
        /// </summary>
        public string CurBuildingName;
        /// <summary>
        /// 当前的物体定义
        /// </summary>
        public ThingDef CurBuildingDef;
        /// <summary>
        /// 是否正处于建造模式中
        /// </summary>
        public bool OnBuildMode;
        /// <summary>
        /// 当前鼠标上的蓝图预览
        /// </summary>
        public GameObject MouseIndicator;

        public BuildingModeTool()
        {

        }

        /// <summary>
        /// Start
        /// </summary>
        /// <param name="def">建筑的定义</param>
        public void BuildStart(ThingDef def)
        {
            CurBuildingDef = def;
            CurBuildingName = def.Name;

            // 获取当前地图的TileMap
            Tilemap main = MapManager.Instance.CurMapMainTilemap;

            // 配置鼠标指示器信息
            OnBuildMode = true;
            MouseIndicator = UnityEngine.Object.Instantiate(CurBuildingDef.Prefab);
            if (!MouseIndicator.GetComponent<Collider2D>())
            {
                Debug.LogError("ERROR: MouseIndicator no Collider2D");
                GameObject.Destroy(MouseIndicator);
                return;
            }
            MouseIndicator.SetActive(true);

        }

        /// <summary>
        /// Update，监听鼠标信息，放置蓝图等
        /// </summary>
        public void BuildUpdate()
        {
            if (OnBuildMode && MouseIndicator != null)
            {
                // 监听鼠标位置信息转换成世界坐标
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Tilemap curTilemap = MapManager.Instance.CurMapMainTilemap;
                Vector3Int cellPosition = curTilemap.WorldToCell(mousePosition);
                Vector3 placePosition = curTilemap.CellToWorld(cellPosition) + new Vector3(0.5f, 0.5f);
                MouseIndicator.transform.position = placePosition;

                SpriteRenderer sr = MouseIndicator.GetComponent<SpriteRenderer>();
                // 如果能建造则设置主体为绿色，否则为红色
                if (MapManager.Instance.ContainsObstaclesList(MouseIndicator))
                {
                    sr.color = Color.green;
                    // 放置
                    if (Input.GetMouseButtonDown(0))
                    {
                        TryPlaced(placePosition);
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

        /// <summary>
        /// End
        /// </summary>
        public void BuildEnd()
        {
            OnBuildMode = false;
            MouseIndicator.SetActive(false);
            UnityEngine.Object.Destroy(MouseIndicator);
        }

        protected void TryPlaced(Vector3 placePosition)
        {
            GameObject newObject = UnityEngine.Object.Instantiate(CurBuildingDef.Prefab,
                                                      placePosition,
                                                      MouseIndicator.transform.rotation,
                                                      BuildingSystemManager.Instance.transform);
            
            MapManager.Instance.AddToObstaclesList(newObject);
            ThingBase Thing = newObject.GetComponent<ThingBase>();
            Thing.OnPlaced();
            //Todo
        }
    }
}