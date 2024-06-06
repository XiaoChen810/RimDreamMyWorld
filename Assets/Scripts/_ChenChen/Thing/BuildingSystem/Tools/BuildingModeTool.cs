using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using ChenChen_Map;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace ChenChen_Thing
{
    /// <summary>
    /// 打开建造模式，放置建筑什么的
    /// </summary>
    public class BuildingModeTool
    {
        public ThingSystemManager BuildingSystemManager;
        /// <summary>
        /// 当前蓝图的名字
        /// </summary>
        public string CurBuildingName;
        /// <summary>
        /// 当前的物体定义
        /// </summary>
        public ThingDef CurBuildingDef;
        /// <summary>
        /// 当前物体的ThingBase组件
        /// </summary>
        public ThingBase CurBuildingBase;
        /// <summary>
        /// 是否正处于建造模式中
        /// </summary>
        public bool OnBuildMode { get; private set; }
        /// <summary>
        /// 当前鼠标上的预览
        /// </summary>
        public GameObject MouseIndicator;

        public BuildingModeTool(ThingSystemManager buildingSystemManager)
        {
            BuildingSystemManager = buildingSystemManager;
        }

        public static readonly string mouseIndicator_string = "MouseIndicator";
        /// <summary>
        /// Start
        /// </summary>
        /// <param name="def">建筑的定义</param>
        public void BuildStart(ThingDef def)
        {
            CurBuildingDef = def;
            CurBuildingName = def.DefName;
            MouseIndicator = UnityEngine.Object.Instantiate(CurBuildingDef.Prefab);
            MouseIndicator.name = mouseIndicator_string;
            MouseIndicator.SetActive(true);
            CurBuildingBase = MouseIndicator.GetComponent<ThingBase>();
            OnBuildMode = true;
        }

        /// <summary>
        /// Update，监听鼠标信息，放置蓝图等
        /// </summary>
        public void BuildUpdate()
        {
            if (OnBuildMode && MouseIndicator != null)
            {
                // 监听鼠标位置信息转换成世界坐标, 并且取整为网格坐标
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int placePosition = StaticFuction.VectorTransToInt(mousePosition);
                MouseIndicator.transform.position = placePosition + new Vector3(CurBuildingDef.Offset.x, CurBuildingDef.Offset.y);

                // 如果能建造则设置主体为绿色，否则为红色
                SpriteRenderer sr = CurBuildingBase.SR;
                if (CurBuildingBase.CanBuildHere())
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
            CurBuildingBase = null;
            CurBuildingDef = null;
            CurBuildingName = null;
        }
    }
}