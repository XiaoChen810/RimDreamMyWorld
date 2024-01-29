using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using 地图生成;

namespace 建筑系统
{
    public class BuildingSystem : MonoBehaviour
    {
        // 瓦片地图，给其他类调用的
        [HideInInspector] public Tilemap BuildingTilemap;
        [HideInInspector] public Tilemap WallTilemap;

        [Header("当前建造的物品名字")]
        public string BuildingName;
        private BlueprintData blueprintData;

        [Header("开启建造模式")]
        public bool BuildingMode;
        /// <summary>
        ///  当切换当前蓝图数据时调用的事件
        /// </summary>
        public Action<string> OnToggleBlueprint;

        [Header("鼠标指示器")]
        public GameObject MouseIndicator;

        Action<Vector3> OnPlace, OnCancel;

        private void Start()
        {
            OnToggleBlueprint += BuildStart;
        }

        private void OnDestroy()
        {
            OnToggleBlueprint -= BuildStart;
        }

        private void Update()
        {
            // 当建造模式开关打开时，进行一次获取蓝图数据和配置鼠标指示器信息
            if (BuildingMode)
            {
                BuildingMode = false;
                ToggleBlueprint(BuildingName);
            }

            BuildUpdate();


        }

        #region  Public

        public void ToggleBlueprint(string name)
        {
            OnToggleBlueprint?.Invoke(name);
        }

        #endregion


        private void BuildStart(string name)
        {
            //  找到当前应该放置的蓝图
            blueprintData = BuildingSystemManager.Instance.GetData(name);
            if (blueprintData == null)
            {
                Debug.LogWarning("不存在此蓝图");
                BuildEnd();
                return;
            }
            else
            {
                // 获取TileMap
                BuildingTilemap = MapManager.Instance.GetChildObject("Building").GetComponent<Tilemap>();
                WallTilemap = MapManager.Instance.GetChildObject("Wall").GetComponent<Tilemap>();

                // 配置鼠标指示器信息
                MouseIndicator.SetActive(true);
                SpriteRenderer sr = MouseIndicator.GetComponent<SpriteRenderer>();
                sr.sprite = blueprintData.PreviewSprite;
            }
        }

        private void BuildUpdate()
        {
            if (MouseIndicator.activeSelf)
            {
                // 监听鼠标位置信息转换成世界坐标
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPosition = BuildingTilemap.WorldToCell(mousePosition);
                Vector3 placePosition = BuildingTilemap.CellToWorld(cellPosition) + new Vector3(0.5f, 0.5f);
                MouseIndicator.transform.position = cellPosition + new Vector3(0.5f, 0.5f);

                // 放置和取消
                if (Input.GetMouseButtonDown(0))
                {
                    OnPlace?.Invoke(cellPosition);
                    GameObject newObject = Instantiate(blueprintData.Prefab, placePosition, Quaternion.identity, this.transform);                   
                    BuildingBlueprintBase blueprint = newObject.GetComponent<BuildingBlueprintBase>();
                    if(blueprint.Name != blueprintData.name)
                    {
                        Debug.LogWarning($"发现蓝图数据与蓝图有所冲突 {blueprint.Name} : {blueprintData.name}");
                    }
                    blueprint.Placed();
                }
                if (Input.GetMouseButtonDown(1))
                {
                    OnCancel?.Invoke(cellPosition);
                    BuildEnd();
                }
            }
        }

        private void BuildEnd()
        {
            BuildingMode = false;
            MouseIndicator.SetActive(false);
        }
    }
}