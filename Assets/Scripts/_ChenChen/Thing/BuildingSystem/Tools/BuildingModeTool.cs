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
        private string _curBuildingName;
        /// <summary>
        /// 当前的物体定义
        /// </summary>
        private ThingDef _curBuildingDef;
        /// <summary>
        /// 当前物体的ThingBase组件
        /// </summary>
        private Thing _curBuildingBase;
        /// <summary>
        /// 是否正处于建造模式中
        /// </summary>
        public bool OnBuildMode { get; private set; }
        /// <summary>
        /// 当前鼠标上的预览
        /// </summary>
        public GameObject _mouseIndicator;

        public BuildingModeTool(ThingSystemManager buildingSystemManager)
        {
            BuildingSystemManager = buildingSystemManager;
        }

        /// <summary>
        /// Start
        /// </summary>
        /// <param name="def">建筑的定义</param>
        public void BuildStart(ThingDef def)
        {
            if(OnBuildMode)
            {
                BuildEnd();
            }
            _curBuildingName = def.DefName;
            _curBuildingDef = def;
            _mouseIndicator = UnityEngine.Object.Instantiate(_curBuildingDef.Prefab);
            _mouseIndicator.name = "MouseIndicator";
            _mouseIndicator.GetComponent<SpriteRenderer>().sortingLayerName = "Above";
            _curBuildingBase = _mouseIndicator.GetComponent<Thing>();
            OnBuildMode = true;
        }

        /// <summary>
        /// Update，监听鼠标信息，放置蓝图等
        /// </summary>
        public void BuildUpdate()
        {
            if (OnBuildMode && _mouseIndicator != null)
            {
                // 监听鼠标位置信息转换成世界坐标, 并且取整为网格坐标
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int placePosition = StaticFuction.VectorTransToInt(mousePosition);
                _mouseIndicator.transform.position = placePosition + new Vector3(_curBuildingDef.Offset.x, _curBuildingDef.Offset.y);

                // 如果能建造则设置主体为绿色，否则为红色
                SpriteRenderer sr = _curBuildingBase.SR;
                if (_curBuildingBase.CanBuildHere())
                {
                    sr.color = Color.green;
                    // 当鼠标按下, 再判断一次能否建造，然后建造
                    if (Input.GetMouseButton(0) && _curBuildingBase.CanBuildHere())
                    {
                        Vector2 posInt = new Vector2(placePosition.x,placePosition.y);
                        Quaternion rot = _mouseIndicator.transform.rotation;
                        BuildingSystemManager.TryGenerateThing(_curBuildingDef, posInt, rot);
                    }
                }
                else
                {
                    sr.color = Color.red;
                }

                // 旋转
                if (_curBuildingDef.CanRotation)
                {
                    // 当按下Q键，物体顺时针旋转90度
                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        _mouseIndicator.transform.Rotate(Vector3.forward, 90f);
                    }
                    // 当按下E键，物体逆时针旋转90度
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        _mouseIndicator.transform.Rotate(Vector3.forward, -90f);
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
            UnityEngine.Object.Destroy(_mouseIndicator);
            _curBuildingBase = null;
            _curBuildingDef = null;
            _curBuildingName = null;
        }
    }
}