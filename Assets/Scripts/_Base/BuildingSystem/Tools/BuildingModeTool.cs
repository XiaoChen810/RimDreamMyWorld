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
        public BuildingSystemManager BuildingSystemManager;
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
        /// 当前鼠标上的预览
        /// </summary>
        public GameObject MouseIndicator;

        public BuildingModeTool(BuildingSystemManager buildingSystemManager)
        {
            BuildingSystemManager = buildingSystemManager;
        }

        /// <summary>
        /// Start
        /// </summary>
        /// <param name="def">建筑的定义</param>
        public void BuildStart(ThingDef def)
        {
            CurBuildingDef = def;
            CurBuildingName = def.DefName;

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
                Vector3 placePosition = curTilemap.CellToWorld(cellPosition);
                MouseIndicator.transform.position = placePosition + new Vector3(CurBuildingDef.offset.x, CurBuildingDef.offset.y);

                SpriteRenderer sr = MouseIndicator.GetComponent<SpriteRenderer>();
                // 如果能建造则设置主体为绿色，否则为红色
                if (CanBuildHere(MouseIndicator))
                {
                    sr.color = Color.green;
                    // 放置
                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector2 posInt = new Vector2(placePosition.x,placePosition.y);
                        Quaternion rot = MouseIndicator.transform.rotation;
                        BuildingSystemManager.TryGenerateThing(CurBuildingDef, posInt, rot, 0, MapManager.Instance.CurrentMapName);
                    }
                }
                else
                {
                    sr.color = Color.red;
                }

                // 旋转
                if (CurBuildingDef.CanRotation)
                {
                    // 当按下Q键，物体顺时针旋转90度
                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        MouseIndicator.transform.Rotate(Vector3.forward, 90f);
                    }
                    // 当按下E键，物体逆时针旋转90度
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        MouseIndicator.transform.Rotate(Vector3.forward, -90f);
                    }
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

        // 检查是否能够在指定位置放置指定的游戏对象
        public bool CanBuildHere(GameObject objectToBuild)
        {
            // 获取待放置对象的 Collider2D 组件
            Collider2D collider = objectToBuild.GetComponent<Collider2D>();

            // 如果待放置对象没有 Collider2D 组件，则返回 false
            if (collider == null)
            {
                Debug.LogWarning("Object to build does not have a Collider2D component.");
                return false;
            }

            // 获取待放置对象 Collider2D 的边界框信息
            Bounds bounds = collider.bounds;

            // 执行碰撞检测，只检测指定图层的碰撞器
            Collider2D[] colliders = Physics2D.OverlapBoxAll(objectToBuild.transform.position, bounds.size, 0f);

            // 遍历检测到的碰撞器，如果有任何一个碰撞器存在，则返回 false，表示无法放置游戏对象
            foreach (Collider2D otherCollider in colliders)
            {
                if (otherCollider.gameObject != objectToBuild) // 忽略待放置游戏对象的碰撞
                {
                    return false;
                }
            }

            // 如果没有任何碰撞器存在，则表示可以放置游戏对象
            return true;
        }
    }
}